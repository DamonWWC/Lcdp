namespace Hjmos.Lcdp.Common
{
    /// <summary>
    /// Represents Event parameters.
    /// </summary>
    /// <remarks>
    /// This class can be used to pass object parameters during event firing.
    /// </remarks>
    public class EventParameters : ParametersBase, IEventParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventParameters"/> class.
        /// </summary>
        public EventParameters() : base() { }

        /// <summary>
        /// Constructs a list of parameters.
        /// </summary>
        /// <param name="query">Query string to be parsed.</param>
        public EventParameters(string query) : base(query) { }
    }
}
