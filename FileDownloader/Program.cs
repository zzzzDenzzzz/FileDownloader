var progress = new Progress<double>(p =>
{
    Console.WriteLine(Math.Round(p, 2));
});

await FileDownloader.FileDownloader.DownloadFile(
    "https://github.com/rodion-m/SystemProgrammingCourse2022/raw/master/files/payments_19mb.zip",
    progress);
