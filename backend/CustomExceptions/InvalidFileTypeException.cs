namespace backend.CustomExceptions
{
    public class InvalidFileTypeException : IOException
    {
        private string path;
        private string acceptedTypeMask;

        public InvalidFileTypeException(string path, string acceptedTypeMask)
        {
            this.path = path;
            this.acceptedTypeMask = acceptedTypeMask;
        }

        public override string Message
        {
            get { return string.Format("File type '{0}' does not fall within the expected range: '{1}'", path, acceptedTypeMask); }
        }
    }
}
