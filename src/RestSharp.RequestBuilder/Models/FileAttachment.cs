using System.IO;

namespace RestSharp.RequestBuilder.Models
{
    /// <summary>
    /// Abstract base class for file attachments with discriminated union pattern.
    /// </summary>
    public abstract class FileAttachment
    {
        /// <summary>
        /// Gets the parameter name for the file attachment.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the content type of the file attachment.
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAttachment"/> class.
        /// </summary>
        /// <param name="name">The parameter name for the file attachment.</param>
        /// <param name="contentType">The content type of the file attachment.</param>
        protected FileAttachment(string name, string contentType)
        {
            Name = name;
            ContentType = contentType;
        }
    }

    /// <summary>
    /// Represents a file attachment from a file system path.
    /// </summary>
    public sealed class PathFileAttachment : FileAttachment
    {
        /// <summary>
        /// Gets the file path.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathFileAttachment"/> class.
        /// </summary>
        /// <param name="name">The parameter name for the file attachment.</param>
        /// <param name="path">The file path.</param>
        /// <param name="contentType">The content type of the file attachment.</param>
        public PathFileAttachment(string name, string path, string contentType = null)
            : base(name, contentType)
        {
            Path = path;
        }
    }

    /// <summary>
    /// Represents a file attachment from a byte array.
    /// </summary>
    public sealed class ByteFileAttachment : FileAttachment
    {
        /// <summary>
        /// Gets the byte array containing the file data.
        /// </summary>
        public byte[] Bytes { get; }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteFileAttachment"/> class.
        /// </summary>
        /// <param name="name">The parameter name for the file attachment.</param>
        /// <param name="bytes">The byte array containing the file data.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="contentType">The content type of the file attachment.</param>
        public ByteFileAttachment(string name, byte[] bytes, string fileName, string contentType = null)
            : base(name, contentType)
        {
            Bytes = bytes;
            FileName = fileName;
        }
    }

    /// <summary>
    /// Represents a file attachment from a stream.
    /// </summary>
    public sealed class StreamFileAttachment : FileAttachment
    {
        /// <summary>
        /// Gets the stream containing the file data.
        /// </summary>
        public Stream Stream { get; }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamFileAttachment"/> class.
        /// </summary>
        /// <param name="name">The parameter name for the file attachment.</param>
        /// <param name="stream">The stream containing the file data.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="contentType">The content type of the file attachment.</param>
        public StreamFileAttachment(string name, Stream stream, string fileName, string contentType = null)
            : base(name, contentType)
        {
            Stream = stream;
            FileName = fileName;
        }
    }
}
