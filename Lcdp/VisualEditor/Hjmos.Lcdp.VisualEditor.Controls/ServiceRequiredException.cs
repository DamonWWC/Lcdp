using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// 用于设计器失败的异常类
    /// </summary>
    [Serializable]
    public class ServiceRequiredException : DesignerException
    {
        /// <summary>获取丢失的服务</summary>
        public Type ServiceType { get; private set; }

        public ServiceRequiredException(Type serviceType) : base($"Service {serviceType.FullName} is required.") => this.ServiceType = serviceType;

        public ServiceRequiredException() { }

        public ServiceRequiredException(string message) : base(message) { }

        public ServiceRequiredException(string message, Exception innerException) : base(message, innerException) { }

        protected ServiceRequiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info is null) throw new ArgumentNullException("info");
            this.ServiceType = info.GetValue("ServiceType", typeof(Type)) as Type;
        }

        /// <inheritdoc/>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null) throw new ArgumentNullException("info");
            base.GetObjectData(info, context);
            info.AddValue("ServiceType", this.ServiceType);
        }
    }
}
