namespace Hjmos.Lcdp.Common
{
    /// <summary>
    /// Represents Event parameters.
    /// </summary>
    /// <remarks>
    /// This class can be used to pass object parameters during event firing.
    /// </remarks>
    public class PageParameters : ParametersBase, IPageParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageParameters"/> class.
        /// </summary>
        public PageParameters() : base() { }

        /// <summary>
        /// Constructs a list of parameters.
        /// </summary>
        /// <param name="query">Query string to be parsed.</param>
        public PageParameters(string query) : base(query) { }
    }
}
