using System;
using System.Collections.Generic;
using UnityShaderParser.Common;

namespace UnityShaderParser.ShaderLab
{
    public abstract class ShaderLabSyntaxVisitor
    {
        protected void DefaultVisit(ShaderLabSyntaxNode node)
        {
            foreach (var child in node.Children)
            {
                child.Accept(this);
            }
        }

        public void VisitMany(IEnumerable<ShaderLabSyntaxNode> nodes)
        {
            foreach (ShaderLabSyntaxNode node in nodes)
            {
                Visit(node);
            }
        }

        public void VisitMany<T>(IList<T> nodes, Action runBetween)
            where T : ShaderLabSyntaxNode
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                Visit(nodes[i]);
                if (i < nodes.Count - 1)
                    runBetween();
            }
        }

        public virtual void Visit(ShaderLabSyntaxNode node) => node?.Accept(this);
        public virtual void VisitShaderNode(ShaderNode node) => DefaultVisit(node);
        public virtual void VisitShaderPropertyNode(ShaderPropertyNode node) => DefaultVisit(node);
        public virtual void VisitShaderPropertyValueFloatNode(ShaderPropertyValueFloatNode node) => DefaultVisit(node);
        public virtual void VisitShaderPropertyValueIntegerNode(ShaderPropertyValueIntegerNode node) => DefaultVisit(node);
        public virtual void VisitShaderPropertyValueVectorNode(ShaderPropertyValueVectorNode node) => DefaultVisit(node);
        public virtual void VisitShaderPropertyValueColorNode(ShaderPropertyValueColorNode node) => DefaultVisit(node);
        public virtual void VisitShaderPropertyValueTextureNode(ShaderPropertyValueTextureNode node) => DefaultVisit(node);
        public virtual void VisitSubShaderNode(SubShaderNode node) => DefaultVisit(node);
        public virtual void VisitShaderPassNode(ShaderPassNode node) => DefaultVisit(node);
        public virtual void VisitShaderCodePassNode(ShaderCodePassNode node) => DefaultVisit(node);
        public virtual void VisitShaderGrabPassNode(ShaderGrabPassNode node) => DefaultVisit(node);
        public virtual void VisitShaderUsePassNode(ShaderUsePassNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandNode(ShaderLabCommandNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandTagsNode(ShaderLabCommandTagsNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandLodNode(ShaderLabCommandLodNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabBasicToggleCommandNode(ShaderLabBasicToggleCommandNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandLightingNode(ShaderLabCommandLightingNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandSeparateSpecularNode(ShaderLabCommandSeparateSpecularNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandZWriteNode(ShaderLabCommandZWriteNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandAlphaToMaskNode(ShaderLabCommandAlphaToMaskNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandZClipNode(ShaderLabCommandZClipNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandConservativeNode(ShaderLabCommandConservativeNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandCullNode(ShaderLabCommandCullNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandZTestNode(ShaderLabCommandZTestNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandBlendNode(ShaderLabCommandBlendNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandOffsetNode(ShaderLabCommandOffsetNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandColorMaskNode(ShaderLabCommandColorMaskNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandAlphaTestNode(ShaderLabCommandAlphaTestNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandFogNode(ShaderLabCommandFogNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandNameNode(ShaderLabCommandNameNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandBindChannelsNode(ShaderLabCommandBindChannelsNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandColorNode(ShaderLabCommandColorNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandBlendOpNode(ShaderLabCommandBlendOpNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandMaterialNode(ShaderLabCommandMaterialNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandSetTextureNode(ShaderLabCommandSetTextureNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandColorMaterialNode(ShaderLabCommandColorMaterialNode node) => DefaultVisit(node);
        public virtual void VisitShaderLabCommandStencilNode(ShaderLabCommandStencilNode node) => DefaultVisit(node);
    }
}
