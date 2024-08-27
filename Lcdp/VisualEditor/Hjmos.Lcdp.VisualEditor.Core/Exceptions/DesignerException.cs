using System;
using System.Runtime.Serialization;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 用于设计器失败的异常类。
    /// </summary>
    [Serializable]
    public class DesignerException : Exception
    {
        public DesignerException() { }

        public DesignerException(string message) : base(message) { }

        public DesignerException(string message, Exception innerException) : base(message, innerException) { }

        protected DesignerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
