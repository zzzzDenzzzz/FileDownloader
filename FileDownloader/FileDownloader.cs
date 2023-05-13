namespace FileDownloader
{
    internal class FileDownloader
    {
        public static async Task DownloadFile(string uri, IProgress<double> progress)
        {
            ArgumentNullException.ThrowIfNull(uri);
            if (!Uri.TryCreate(uri, UriKind.Absolute, out var uriResult))
            {
                throw new ArgumentException("Invalid uri", nameof(uri));
            }

            using var client = new HttpClient();
            try
            {
                using var response = await client.GetAsync(
                uriResult, HttpCompletionOption.ResponseHeadersRead).WaitAsync(TimeSpan.FromSeconds(10));

                var contentLength = response.Content.Headers.ContentLength;

                await using var contentStream = await response.Content.ReadAsStreamAsync();

                byte[] buffer = new byte[8192];
                var totalBytesRead = 0L;
                await using var file = File.OpenWrite("file.zip");

                while (true)
                {
                    int countOfBytesRead = await contentStream.ReadAsync(buffer);
                    if (countOfBytesRead == 0)
                    {
                        break;
                    }

                    if (countOfBytesRead == buffer.Length)
                    {
                        await file.WriteAsync(buffer);
                    }
                    else
                    {
                        var bytesRead = buffer[..countOfBytesRead];
                        await file.WriteAsync(bytesRead);
                    }
                    totalBytesRead += countOfBytesRead;

                    if (contentLength != null)
                    {
                        progress.Report((double)totalBytesRead / contentLength.Value);
                    }
                }
            }
            catch (TimeoutException ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }
    }
}
