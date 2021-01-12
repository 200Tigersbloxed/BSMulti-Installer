using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BSMulti_Installer2.Utilities
{
    /// <summary>
    /// From: https://stackoverflow.com/a/43169927
    /// </summary>
    public class HttpClientDownloadWithProgress : IDisposable
    {
        private readonly Uri _downloadUri;
        private readonly string _destinationFilePath;
        private readonly int ReportRate;

        private HttpClient _httpClient;
        private bool disposedValue;
        public bool AddExtensionToPath { get; set; } = true;
        private string FileExtension { get; set; }

        public HttpClient WebClient
        {
            get { return _httpClient; }
            protected set
            {
                _httpClient = value;
            }
        }

        public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage);

        public event ProgressChangedHandler ProgressChanged;

        public HttpClientDownloadWithProgress(HttpClient client, Uri downloadUri, string destinationFilePath, int reportRate = 50)
        {
            ReportRate = Math.Min(reportRate, 1);
            if (client == null)
                throw new ArgumentNullException(nameof(client), "client cannot be null for HttpClientDownloadWithProgress.");
            if (string.IsNullOrEmpty(destinationFilePath?.Trim()))
                throw new ArgumentNullException(nameof(destinationFilePath), "destinationFilePath cannot be null or empty for HttpClientDownloadWithProgress.");
            WebClient = client;
            _downloadUri = downloadUri ?? throw new ArgumentNullException(nameof(downloadUri), "downloadUri cannot be null for HttpClientDownloadWithProgress.");
            _destinationFilePath = destinationFilePath;
        }

        public HttpClientDownloadWithProgress(HttpClient client, string downloadUrl, string destinationFilePath, int reportRate = 50)
        {
            ReportRate = Math.Min(reportRate, 1);
            if (string.IsNullOrEmpty(destinationFilePath?.Trim()))
                throw new ArgumentNullException(nameof(destinationFilePath), "destinationFilePath cannot be null or empty for HttpClientDownloadWithProgress.");
            if (string.IsNullOrEmpty(downloadUrl?.Trim()))
                throw new ArgumentNullException(nameof(downloadUrl), "destinationFilePath cannot be null or empty for HttpClientDownloadWithProgress.");

            WebClient = client ?? throw new ArgumentNullException(nameof(client), "client cannot be null for HttpClientDownloadWithProgress.");
            _downloadUri = new Uri(downloadUrl);
            _destinationFilePath = destinationFilePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns></returns>
        public async Task<string> StartDownload(CancellationToken cancellationToken)
        {
            //_httpClient = new HttpClient { Timeout = TimeSpan.FromDays(1) };

            using (var response = await _httpClient.GetAsync(_downloadUri, cancellationToken).ConfigureAwait(false))
                return await DownloadFileFromHttpResponseMessage(response, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns></returns>
        private async Task<string> DownloadFileFromHttpResponseMessage(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers?.ContentLength ?? 0;
            if (AddExtensionToPath)
            {
                string requestUrl = response.RequestMessage.RequestUri.ToString();
                if (requestUrl.EndsWith("dll", StringComparison.OrdinalIgnoreCase))
                    FileExtension = "dll";
                else if (requestUrl.EndsWith("zip", StringComparison.OrdinalIgnoreCase))
                    FileExtension = "zip";
                else
                {
                    string mediaType = response.Content.Headers?.ContentType.MediaType;
                    int startIndex = mediaType.LastIndexOf('/');
                    if (startIndex > 0)
                        FileExtension = mediaType.Substring(startIndex + 1);
                    if (FileExtension == "x-msdos-program")
                        FileExtension = "dll";
                }
            }
            using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                return await ProcessContentStream(totalBytes, contentStream, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalDownloadSize"></param>
        /// <param name="contentStream"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns></returns>
        private async Task<string> ProcessContentStream(long? totalDownloadSize, Stream contentStream, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var totalBytesRead = 0L;
            var readCount = 0L;
            var buffer = new byte[8192];
            var isMoreToRead = true;
            string destination = _destinationFilePath;
            if (AddExtensionToPath && !string.IsNullOrEmpty(FileExtension))
                destination = _destinationFilePath + "." + FileExtension;
            var file = new FileInfo(destination);
            file.Directory.Create();
            using (var fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                do
                {
                    var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                    if (bytesRead == 0)
                    {
                        isMoreToRead = false;
                        TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                        continue;
                    }

                    await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);

                    totalBytesRead += bytesRead;
                    readCount += 1;

                    if (ReportRate == 1 || readCount % ReportRate == 0)
                        TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                }
                while (isMoreToRead);
            }
            return destination;
        }

        private void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead)
        {
            if (ProgressChanged == null)
                return;

            double? progressPercentage = null;
            if (totalDownloadSize.HasValue)
                progressPercentage = Math.Round((double)totalBytesRead / (double)totalDownloadSize.Value, 4);
            ProgressChanged(totalDownloadSize, totalBytesRead, progressPercentage);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~HttpClientDownloadWithProgress()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
