﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using dnlib.DotNet;
using dnlib.Utils;

namespace dnEditor.Handlers
{
    public static class TypeHandler
    {
        public static void ProcessTypeMembers(TreeNode parentNode, ref List<TreeNode> children)
        {
            var type = parentNode.Tag as TypeDef;

            foreach (TypeDef nestedType in type.NestedTypes)
            {
                TreeNode newTypeNode = TreeViewHandler.NewType(nestedType);
                
                var subChildren = new List<TreeNode>();
                ProcessTypeMembers(newTypeNode, ref subChildren);

                foreach (var child in subChildren)
                    newTypeNode.Nodes.Add(child);

                children.Add(newTypeNode);
            }
                

            foreach (MethodDef method in type.Methods)
            {
                List<MethodDef> accessorMethods = type.GetAccessorMethods();

                if (!accessorMethods.Contains(method))
                    children.Add(TreeViewHandler.NewMethod(method));
            }

            foreach (PropertyDef property in type.Properties)
                children.Add(TreeViewHandler.NewProperty(property));

            foreach (FieldDef field in type.Fields)
                children.Add(TreeViewHandler.NewField(field));

            foreach (EventDef @event in type.Events)
                children.Add(TreeViewHandler.NewEvent(@event));
        }

        public static void HandleType(TypeDef type)
        {
            TreeNode targetType = TreeViewHandler.NewType(type);

            var children = new List<TreeNode>();
            ProcessTypeMembers(targetType, ref children);

            foreach (var child in children)
                targetType.Nodes.Add(child);

            if (!TreeViewHandler.NameSpaceList.Contains(type.Namespace))
            {
                TreeNode nameSpace = TreeViewHandler.NewNameSpace(type.Namespace);

                TreeViewHandler.NameSpaceList.Add(type.Namespace);

                TreeViewHandler.CurrentModule.TreeView.BeginInvoke(new MethodInvoker(() =>
                {
                    nameSpace.AddTo(TreeViewHandler.CurrentModule);
                    targetType.AddTo(nameSpace);
                }));

            }
            else
            {
                TreeViewHandler.CurrentModule.TreeView.BeginInvoke(new MethodInvoker(() => targetType.AddTo(TreeViewHandler.CurrentModule.Nodes.Cast<TreeNode>().First(n => n.Text == type.Namespace))));
            }
        }
    }
}