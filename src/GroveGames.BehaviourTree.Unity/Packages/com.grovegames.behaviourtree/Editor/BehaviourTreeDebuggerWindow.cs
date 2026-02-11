using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace GroveGames.BehaviourTree.Unity.Editor
{
    public sealed class BehaviourTreeDebuggerWindow : EditorWindow
    {
        private const float LEFT_PANEL_WIDTH = 250f;
        private const float NODE_WIDTH = 180f;
        private const float NODE_HEIGHT = 40f;
        private const float HORIZONTAL_SPACING = 60f;
        private const float VERTICAL_SPACING = 20f;

        private List<BehaviourTreeMono> _controllers = new();
        private BehaviourTreeMono _selectedController;
        private Vector2 _leftPanelScroll;
        private Vector2 _treePanelScroll;

        private GUIStyle _nodeStyleRunning;
        private GUIStyle _nodeStyleSuccess;
        private GUIStyle _nodeStyleFailure;
        private GUIStyle _nodeStyleDefault;
        private GUIStyle _selectedButtonStyle;

        private bool _stylesInitialized;

        [MenuItem("Tools/GroveGames/Behaviour Tree Debugger")]
        public static void Open()
        {
            var window = GetWindow<BehaviourTreeDebuggerWindow>("BT Debugger");
            window.minSize = new Vector2(800, 400);
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.hierarchyChanged += RefreshControllerList;
            RefreshControllerList();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.hierarchyChanged -= RefreshControllerList;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            RefreshControllerList();
            Repaint();
        }

        private void RefreshControllerList()
        {
            _controllers.Clear();
            var found = FindObjectsByType<BehaviourTreeMono>(FindObjectsSortMode.None);
            _controllers.AddRange(found);

            if (_selectedController != null && !_controllers.Contains(_selectedController))
            {
                _selectedController = null;
            }
        }

        private void InitStyles()
        {
            if (_stylesInitialized)
            {
                return;
            }

            _nodeStyleRunning = CreateNodeStyle(new Color(0.9f, 0.75f, 0.2f));
            _nodeStyleSuccess = CreateNodeStyle(new Color(0.3f, 0.8f, 0.3f));
            _nodeStyleFailure = CreateNodeStyle(new Color(0.9f, 0.3f, 0.3f));
            _nodeStyleDefault = CreateNodeStyle(new Color(0.4f, 0.4f, 0.4f));

            _selectedButtonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                normal = { background = CreateColorTexture(new Color(0.3f, 0.5f, 0.8f)) },
                fontStyle = FontStyle.Bold
            };

            _stylesInitialized = true;
        }

        private GUIStyle CreateNodeStyle(Color bgColor)
        {
            var style = new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 11,
                normal =
                {
                    background = CreateColorTexture(bgColor),
                    textColor = Color.white
                },
                border = new RectOffset(4, 4, 4, 4),
                padding = new RectOffset(8, 8, 4, 4)
            };
            return style;
        }

        private Texture2D CreateColorTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private void OnGUI()
        {
            InitStyles();

            EditorGUILayout.BeginHorizontal();

            DrawLeftPanel();
            DrawTreePanel();

            EditorGUILayout.EndHorizontal();

            if (Application.isPlaying)
            {
                Repaint();
            }
        }

        private void DrawLeftPanel()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(LEFT_PANEL_WIDTH));

            EditorGUILayout.LabelField("Behaviour Trees", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            if (GUILayout.Button("Refresh", GUILayout.Height(25)))
            {
                RefreshControllerList();
            }

            EditorGUILayout.Space(10);

            if (_controllers.Count == 0)
            {
                EditorGUILayout.HelpBox("No BehaviourTreeController found in scene.", MessageType.Info);
            }
            else
            {
                _leftPanelScroll = EditorGUILayout.BeginScrollView(_leftPanelScroll);

                foreach (var controller in _controllers)
                {
                    if (controller == null)
                    {
                        continue;
                    }

                    var isSelected = _selectedController == controller;
                    var style = isSelected ? _selectedButtonStyle : EditorStyles.miniButton;
                    var label = controller.gameObject.name;

                    if (GUILayout.Button(label, style, GUILayout.Height(28)))
                    {
                        _selectedController = controller;
                        Selection.activeGameObject = controller.gameObject;
                    }
                }

                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();

            var rect = GUILayoutUtility.GetLastRect();
            EditorGUI.DrawRect(new Rect(rect.xMax, rect.y, 1, rect.height), new Color(0.2f, 0.2f, 0.2f));
        }

        private void DrawTreePanel()
        {
            EditorGUILayout.BeginVertical();

            if (_selectedController == null)
            {
                EditorGUILayout.HelpBox("Select a BehaviourTreeController from the list.", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to see the behaviour tree visualization.", MessageType.Warning);
                EditorGUILayout.EndVertical();
                return;
            }

            var behaviourTree = _selectedController.Tree;
            if (behaviourTree == null)
            {
                EditorGUILayout.HelpBox("BehaviourTree is not initialized.", MessageType.Warning);
                EditorGUILayout.EndVertical();
                return;
            }

            _treePanelScroll = EditorGUILayout.BeginScrollView(_treePanelScroll);

            var treeData = ExtractTreeData(behaviourTree);
            if (treeData != null)
            {
                DrawTreeVisualization(treeData);
            }
            else
            {
                EditorGUILayout.HelpBox("Could not extract tree data. Check if the BehaviourTree has a valid root.", MessageType.Error);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private NodeData ExtractTreeData(object behaviourTree)
        {
            try
            {
                var rootProperty = behaviourTree.GetType().GetProperty("Root", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (rootProperty == null)
                {
                    return null;
                }

                var root = rootProperty.GetValue(behaviourTree);
                if (root == null)
                {
                    return null;
                }

                return ExtractNodeData(root, 0);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error extracting tree data: {e.Message}");
                return null;
            }
        }

        private NodeData ExtractNodeData(object node, int depth, NodeData parent = null)
        {
            if (node == null)
            {
                return null;
            }

            var nodeData = new NodeData
            {
                Name = GetNodeName(node),
                State = GetNodeState(node),
                Depth = depth,
                Children = new List<NodeData>(),
                Parent = parent
            };

            var children = GetNodeChildren(node);
            if (children != null)
            {
                foreach (var child in children)
                {
                    var childData = ExtractNodeData(child, depth + 1, nodeData);
                    if (childData != null)
                    {
                        nodeData.Children.Add(childData);
                    }
                }
            }

            return nodeData;
        }

        private void CalculateActivePath(NodeData root)
        {
            // First, correct stale states (children of non-running parents should not show as running)
            CorrectStaleStates(root);

            // Reset all nodes
            ResetActivePath(root);

            // Find the deepest running or most recently evaluated node
            var activeLeaf = FindActiveLeaf(root);
            if (activeLeaf == null)
            {
                return;
            }

            // Mark path from active leaf to root
            var current = activeLeaf;
            while (current != null)
            {
                current.IsOnActivePath = true;
                current = current.Parent;
            }

            // Calculate evaluation order
            var order = 1;
            CalculateEvaluationOrder(root, ref order);
        }

        private void CorrectStaleStates(NodeData node, bool parentIsActive = true)
        {
            // If parent is not active (Failure, None, or Success without Running children),
            // then this node's Running state is stale
            if (!parentIsActive && node.State == NodeState.Running)
            {
                node.State = NodeState.None;
            }

            // Determine if this node is "active" for its children
            // A node is active if it's Running, or if it's the root
            var isActiveForChildren = node.State == NodeState.Running || node.Parent == null;

            // For Selector/Sequence that succeeded, only the path that led to success is valid
            // Children showing Running when parent is Success are stale
            if (node.State == NodeState.Success || node.State == NodeState.Failure || node.State == NodeState.None)
            {
                isActiveForChildren = false;
            }

            foreach (var child in node.Children)
            {
                CorrectStaleStates(child, isActiveForChildren);
            }
        }

        private void ResetActivePath(NodeData node)
        {
            node.IsOnActivePath = false;
            node.EvaluationOrder = 0;
            foreach (var child in node.Children)
            {
                ResetActivePath(child);
            }
        }

        private NodeData FindActiveLeaf(NodeData node)
        {
            // Only follow SUCCESS or RUNNING paths - FAILURE means "tried and failed, moved on"
            if (node.State != NodeState.Success && node.State != NodeState.Running)
            {
                return null;
            }

            // If this node is RUNNING, it's the active leaf
            if (node.State == NodeState.Running)
            {
                // But first check if any child is also running (deeper in the tree)
                foreach (var child in node.Children)
                {
                    var result = FindActiveLeaf(child);
                    if (result != null)
                    {
                        return result;
                    }
                }
                // No running child, this is the active leaf
                return node;
            }

            // For SUCCESS nodes, check children for RUNNING or SUCCESS
            foreach (var child in node.Children)
            {
                var result = FindActiveLeaf(child);
                if (result != null)
                {
                    return result;
                }
            }

            // SUCCESS leaf node (no children or all children are None/Failure)
            if (node.Children.Count == 0 || node.Children.TrueForAll(c => c.State == NodeState.None || c.State == NodeState.Failure))
            {
                return node;
            }

            return null;
        }

        private void CalculateEvaluationOrder(NodeData node, ref int order)
        {
            if (node.State != NodeState.None)
            {
                node.EvaluationOrder = order++;
            }

            foreach (var child in node.Children)
            {
                CalculateEvaluationOrder(child, ref order);
            }
        }

        private string GetNodeName(object node)
        {
            var typeName = node.GetType().Name;

            var nameProperty = node.GetType().GetProperty("Name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (nameProperty != null)
            {
                var name = nameProperty.GetValue(node) as string;
                if (!string.IsNullOrEmpty(name))
                {
                    return name;
                }
            }

            return typeName;
        }

        private NodeState GetNodeState(object node)
        {
            // Try property first
            var stateProperty = FindPropertyInHierarchy(node.GetType(), "State");
            if (stateProperty != null)
            {
                var state = stateProperty.GetValue(node);
                return ParseNodeState(state);
            }

            // Try _nodeState field (used in GroveGames.BehaviourTree)
            var nodeStateField = FindFieldInHierarchy(node.GetType(), "_nodeState");
            if (nodeStateField != null)
            {
                var stateValue = nodeStateField.GetValue(node);
                return ParseNodeState(stateValue);
            }

            // Try _state field
            var stateField = FindFieldInHierarchy(node.GetType(), "_state");
            if (stateField != null)
            {
                var stateValue = stateField.GetValue(node);
                return ParseNodeState(stateValue);
            }

            return NodeState.None;
        }

        private NodeState ParseNodeState(object state)
        {
            if (state == null)
            {
                return NodeState.None;
            }

            var stateStr = state.ToString().ToLower();
            return stateStr switch
            {
                "running" => NodeState.Running,
                "success" => NodeState.Success,
                "failure" => NodeState.Failure,
                _ => NodeState.None
            };
        }

        private IEnumerable<object> GetNodeChildren(object node)
        {
            // Try to find _children field (for Composite nodes like Selector, Sequence)
            var childrenField = FindFieldInHierarchy(node.GetType(), "_children");
            if (childrenField != null)
            {
                var children = childrenField.GetValue(node);
                if (children is System.Collections.IEnumerable enumerable)
                {
                    foreach (var child in enumerable)
                    {
                        if (IsValidNode(child))
                        {
                            yield return child;
                        }
                    }
                    yield break;
                }
            }

            // Try to find Children property
            var childrenProperty = FindPropertyInHierarchy(node.GetType(), "Children");
            if (childrenProperty != null)
            {
                var children = childrenProperty.GetValue(node);
                if (children is System.Collections.IEnumerable enumerable)
                {
                    foreach (var child in enumerable)
                    {
                        if (IsValidNode(child))
                        {
                            yield return child;
                        }
                    }
                    yield break;
                }
            }

            // Try to find _child field (for Decorator nodes like Conditional, Succeeder)
            var childField = FindFieldInHierarchy(node.GetType(), "_child");
            if (childField != null)
            {
                var child = childField.GetValue(node);
                if (IsValidNode(child))
                {
                    yield return child;
                }
                yield break;
            }

            // Try to find Child property
            var childProperty = FindPropertyInHierarchy(node.GetType(), "Child");
            if (childProperty != null)
            {
                var child = childProperty.GetValue(node);
                if (IsValidNode(child))
                {
                    yield return child;
                }
            }
        }

        private FieldInfo FindFieldInHierarchy(Type type, string fieldName)
        {
            var currentType = type;
            while (currentType != null && currentType != typeof(object))
            {
                var field = currentType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (field != null)
                {
                    return field;
                }
                currentType = currentType.BaseType;
            }
            return null;
        }

        private PropertyInfo FindPropertyInHierarchy(Type type, string propertyName)
        {
            var currentType = type;
            while (currentType != null && currentType != typeof(object))
            {
                var property = currentType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (property != null)
                {
                    return property;
                }
                currentType = currentType.BaseType;
            }
            return null;
        }

        private bool IsValidNode(object node)
        {
            if (node == null)
            {
                return false;
            }

            // Filter out BehaviourNode.Empty and similar empty/null object patterns
            var typeName = node.GetType().Name;
            if (typeName.Contains("Empty") || typeName.Contains("Null") || typeName == "EmptyNode")
            {
                return false;
            }

            return true;
        }

        private void DrawTreeVisualization(NodeData rootNode)
        {
            // Calculate active evaluation path
            CalculateActivePath(rootNode);

            var positions = new Dictionary<NodeData, Vector2>();
            CalculateNodePositions(rootNode, positions, 0, 0);

            float maxX = 0;
            float maxY = 0;
            foreach (var pos in positions.Values)
            {
                maxX = Mathf.Max(maxX, pos.x + NODE_WIDTH);
                maxY = Mathf.Max(maxY, pos.y + NODE_HEIGHT);
            }

            // Calculate available space and center the tree vertically
            var availableHeight = position.height - 100;
            var treeHeight = maxY + 50;
            var verticalOffset = Mathf.Max(20, (availableHeight - treeHeight) / 2);

            var areaRect = GUILayoutUtility.GetRect(maxX + 50, Mathf.Max(maxY + 50, availableHeight));

            var offsetRect = new Rect(areaRect.x, areaRect.y + verticalOffset, areaRect.width, areaRect.height);
            GUI.BeginGroup(offsetRect);

            // Draw connections - inactive first, then active on top
            foreach (var kvp in positions)
            {
                var node = kvp.Key;
                var pos = kvp.Value;

                foreach (var child in node.Children)
                {
                    if (positions.TryGetValue(child, out var childPos))
                    {
                        var isActivePath = node.IsOnActivePath && child.IsOnActivePath;
                        if (isActivePath) continue; // Draw active paths later

                        DrawConnection(pos, childPos, new Color(0.4f, 0.4f, 0.4f), 2f);
                    }
                }
            }

            // Draw active path connections on top with glow effect
            foreach (var kvp in positions)
            {
                var node = kvp.Key;
                var pos = kvp.Value;

                foreach (var child in node.Children)
                {
                    if (positions.TryGetValue(child, out var childPos))
                    {
                        var isActivePath = node.IsOnActivePath && child.IsOnActivePath;
                        if (!isActivePath) continue;

                        // Glow effect - draw wider line behind
                        var glowColor = new Color(0.3f, 0.8f, 1f, 0.3f);
                        DrawConnection(pos, childPos, glowColor, 8f);

                        // Main active path line
                        var activeColor = new Color(0.3f, 0.8f, 1f);
                        DrawConnection(pos, childPos, activeColor, 3f);
                    }
                }
            }

            // Draw nodes
            foreach (var kvp in positions)
            {
                var node = kvp.Key;
                var pos = kvp.Value;
                var nodeRect = new Rect(pos.x, pos.y, NODE_WIDTH, NODE_HEIGHT);

                var style = node.State switch
                {
                    NodeState.Running => _nodeStyleRunning,
                    NodeState.Success => _nodeStyleSuccess,
                    NodeState.Failure => _nodeStyleFailure,
                    _ => _nodeStyleDefault
                };

                // Draw glow for active path nodes
                if (node.IsOnActivePath && node.State != NodeState.None)
                {
                    var glowRect = new Rect(nodeRect.x - 3, nodeRect.y - 3, nodeRect.width + 6, nodeRect.height + 6);
                    EditorGUI.DrawRect(glowRect, new Color(0.3f, 0.8f, 1f, 0.4f));
                }

                // Pulsing effect for Running nodes
                if (node.State == NodeState.Running)
                {
                    var pulse = Mathf.Sin((float)EditorApplication.timeSinceStartup * 4f) * 0.5f + 0.5f;
                    var pulseRect = new Rect(nodeRect.x - 2 - pulse * 3, nodeRect.y - 2 - pulse * 3,
                                             nodeRect.width + 4 + pulse * 6, nodeRect.height + 4 + pulse * 6);
                    EditorGUI.DrawRect(pulseRect, new Color(1f, 0.9f, 0.2f, 0.3f * pulse));
                }

                GUI.Box(nodeRect, node.Name, style);

                // State indicator
                var indicatorRect = new Rect(nodeRect.x + 5, nodeRect.y + 5, 10, 10);
                var indicatorColor = node.State switch
                {
                    NodeState.Running => new Color(1f, 0.9f, 0.2f),
                    NodeState.Success => new Color(0.2f, 1f, 0.2f),
                    NodeState.Failure => new Color(1f, 0.2f, 0.2f),
                    _ => new Color(0.5f, 0.5f, 0.5f)
                };
                EditorGUI.DrawRect(indicatorRect, indicatorColor);

                // Evaluation order number
                if (node.EvaluationOrder > 0)
                {
                    var orderRect = new Rect(nodeRect.xMax - 20, nodeRect.y + 5, 15, 15);
                    var orderStyle = new GUIStyle(EditorStyles.miniLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        normal = { textColor = Color.white },
                        fontStyle = FontStyle.Bold,
                        fontSize = 9
                    };
                    EditorGUI.DrawRect(orderRect, new Color(0.2f, 0.2f, 0.2f, 0.8f));
                    GUI.Label(orderRect, node.EvaluationOrder.ToString(), orderStyle);
                }
            }

            GUI.EndGroup();
        }

        private void DrawConnection(Vector2 startPos, Vector2 endPos, Color color, float width)
        {
            var startPoint = new Vector3(startPos.x + NODE_WIDTH, startPos.y + NODE_HEIGHT / 2, 0);
            var endPoint = new Vector3(endPos.x, endPos.y + NODE_HEIGHT / 2, 0);
            var midX = (startPoint.x + endPoint.x) / 2;

            Handles.BeginGUI();
            Handles.color = color;
            Handles.DrawAAPolyLine(width,
                startPoint,
                new Vector3(midX, startPoint.y, 0),
                new Vector3(midX, endPoint.y, 0),
                endPoint
            );
            Handles.EndGUI();
        }

        private float CalculateNodePositions(NodeData node, Dictionary<NodeData, Vector2> positions, int depth, float yOffset)
        {
            var x = 20 + depth * (NODE_WIDTH + HORIZONTAL_SPACING);

            if (node.Children.Count == 0)
            {
                positions[node] = new Vector2(x, yOffset);
                return yOffset + NODE_HEIGHT + VERTICAL_SPACING;
            }

            var startY = yOffset;
            var currentY = yOffset;

            foreach (var child in node.Children)
            {
                currentY = CalculateNodePositions(child, positions, depth + 1, currentY);
            }

            var firstChildY = positions[node.Children[0]].y;
            var lastChildY = positions[node.Children[^1]].y;
            var centerY = (firstChildY + lastChildY) / 2;

            positions[node] = new Vector2(x, centerY);

            return currentY;
        }

        private class NodeData
        {
            public string Name;
            public NodeState State;
            public int Depth;
            public List<NodeData> Children;
            public NodeData Parent;
            public bool IsOnActivePath;
            public int EvaluationOrder;
        }

        private enum NodeState
        {
            None,
            Running,
            Success,
            Failure
        }
    }
}
