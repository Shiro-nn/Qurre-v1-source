using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;
namespace Qurre.API.Modules
{
    internal sealed class CommentsOGV : ChainedObjectGraphVisitor
    {
        public CommentsOGV(IObjectGraphVisitor<IEmitter> nextVisitor) : base(nextVisitor) { }
        public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
        {
            if (value is ObjectDescriptor commentsDescriptor && commentsDescriptor.Comment != null)
            {
                context.Emit(new Comment(commentsDescriptor.Comment.Replace("\n", "\n#"), false));
            }

            return base.EnterMapping(key, value, context);
        }
    }
}