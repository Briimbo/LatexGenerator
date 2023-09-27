namespace LatexGenerator.Model
{
    internal interface ITerm
    {
        ITerm Parent { get; set; }

        /// <summary>
        /// Replaces the oldTerm in this component with the newTerm. OldTerm needs to be a direct child of this component. This also updates the Parent field of newTerm.
        /// </summary>
        /// <param name="oldTerm">The term to be replaced (direct child of this term)</param>
        /// <param name="newTerm">The new term that is the replacement of old</param>
        ITerm Replace(ITerm oldTerm, ITerm newTerm);

        string ToLatexCode();
    }
}
