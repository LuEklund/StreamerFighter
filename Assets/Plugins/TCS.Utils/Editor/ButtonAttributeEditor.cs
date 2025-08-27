#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UnityEditor;
namespace TCS.Utils.Editor {
    /// <summary>
    /// Generic custom editor that appends [Button]s to any inspected object.
    /// Works for MonoBehaviours and ScriptableObjects; supports multi-object editing.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor( typeof(UnityEngine.Object), true )]
    public class ButtonAttributeEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            // Draw the default inspector first
            DrawDefaultInspector();

            // Then append any [Button]-decorated methods
            DrawButtonsForTargets();
        }

        private void DrawButtonsForTargets() {
            // We only operate on the *actual* inspected type, not Unity internals
            var inspectedType = target.GetType();

            // Skip Unity built-in inspectors (Transform, etc.)
            if ( inspectedType.Namespace != null &&
                 (inspectedType.Namespace.StartsWith( "UnityEngine" ) || inspectedType.Namespace.StartsWith( "UnityEditor" )) )
                return;

            var methods = inspectedType
                .GetMethods( BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic )
                .Where( m => m.GetCustomAttributes( typeof(ButtonAttribute), true ).Length > 0 )
                .ToArray();

            if ( methods.Length == 0 )
                return;

            EditorGUILayout.Space( 8 );
            EditorGUILayout.LabelField( "BUTTONS", EditorStyles.boldLabel );
            EditorGUILayout.Space( 2 );

            foreach (var method in methods) {
                var attributes = method.GetCustomAttributes( typeof(ButtonAttribute), true )
                    .Cast<ButtonAttribute>();

                foreach (var attr in attributes) {
                    DrawButtonForMethod( method, attr );
                }
            }
        }

        private void DrawButtonForMethod(MethodInfo method, ButtonAttribute attr) {
            // Only support parameterless methods (to keep UX simple)
            if ( method.GetParameters().Length != 0 ) {
                using (new EditorGUI.DisabledScope( true )) {
                    EditorGUILayout.HelpBox( $"{method.Name} has parameters; [Button] only supports parameterless methods.", MessageType.Info );
                }

                return;
            }

            // Mode gating
            bool shouldShow =
                attr.ShowMode == ButtonAttribute.Mode.Always ||
                (attr.ShowMode == ButtonAttribute.Mode.EditorOnly && !Application.isPlaying) ||
                (attr.ShowMode == ButtonAttribute.Mode.PlayModeOnly && Application.isPlaying);

            if ( !shouldShow )
                return;

            // Enable-if logic (optional)
            bool enabledByCondition = true;
            if ( !string.IsNullOrEmpty( attr.EnableIf ) ) {
                enabledByCondition = EvaluateEnableIf( attr.EnableIf, method.DeclaringType, targets );
            }

            float height = attr.Height > 0f ? attr.Height : EditorGUIUtility.singleLineHeight * 1.3f;
            string label = string.IsNullOrWhiteSpace( attr.Label ) ? Nicify( method.Name ) : attr.Label;

            using (new EditorGUI.DisabledScope( !enabledByCondition )) {
                if ( GUILayout.Button( label, GUILayout.Height( height ) ) ) {
                    InvokeOnAllTargets( method, targets );
                }
            }
        }

        private static void InvokeOnAllTargets(MethodInfo method, UnityEngine.Object[] objs) {
            Undo.IncrementCurrentGroup();
            int group = Undo.GetCurrentGroup();

            foreach (var obj in objs) {
                try {
                    if ( !method.IsStatic )
                        Undo.RecordObject( obj, $"Invoke {method.Name}" );

                    object instance = method.IsStatic ? null : obj;
                    method.Invoke( instance, null );

                    // Mark dirty if we may have mutated serialized state
                    if ( !method.IsStatic && obj is UnityEngine.Object uo )
                        EditorUtility.SetDirty( uo );
                }
                catch (TargetInvocationException tie) {
                    Debug.LogException( tie.InnerException ?? tie, obj );
                }
                catch (Exception ex) {
                    Debug.LogException( ex, obj );
                }
            }

            Undo.CollapseUndoOperations( group );
        }

        private static bool EvaluateEnableIf(string memberName, Type declaringType, UnityEngine.Object[] objs) {
            // Evaluate on the *first* target for simplicity/consistency
            var obj = objs != null && objs.Length > 0 ? objs[0] : null;
            object instance = obj;

            // Try bool field
            var field = declaringType.GetField( memberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic );
            if ( field != null && field.FieldType == typeof(bool) )
                return (bool)field.GetValue( field.IsStatic ? null : instance );

            // Try bool property
            var prop = declaringType.GetProperty( memberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic );
            if ( prop != null && prop.PropertyType == typeof(bool) && prop.GetIndexParameters().Length == 0 )
                return (bool)prop.GetValue( prop.GetMethod.IsStatic ? null : instance );

            // Try parameterless bool method
            var method = declaringType.GetMethod( memberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic );
            if ( method != null && method.ReturnType == typeof(bool) && method.GetParameters().Length == 0 )
                return (bool)method.Invoke( method.IsStatic ? null : instance, null );

            // If not found or not bool, default to enabled
            return true;
        }

        private static string Nicify(string methodName) {
            // Convert "DoSuperThingNow" -> "Do Super Thing Now"
            // Respect underscores too
            string s = System.Text.RegularExpressions.Regex.Replace( methodName, "([a-z])([A-Z])", "$1 $2" );
            s = s.Replace( "_", " " );
            // Title case-ish: first char uppercase, leave rest as-is for readability
            return s.Trim();
        }
    }
}
#endif