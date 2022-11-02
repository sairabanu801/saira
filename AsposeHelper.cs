using Aspose.Words;
using Aspose.Words.Rendering;
using Aspose.Words.Saving;
using System;
using System.IO;

namespace myNAVSales.myNAVPortal.CrossCutting
{
    public static class AsposeHelper
    {
        #region public Methods

        public static string CreateThumbnails(Byte[] blobStream, string blobName, string CurrentDirPath)
        {
            string fileName = string.Empty;
            try
            {
                Stream stream = new MemoryStream(blobStream);
                MemoryStream thumbailstream = new MemoryStream();

                SetAsposeLicence();

                var doc = new Aspose.Words.Document(stream);
                ImageSaveOptions options = new ImageSaveOptions(Aspose.Words.SaveFormat.Png);
                options.PageIndex = 0;
                options.PageCount = 1;
                MemoryStream imgStream = new MemoryStream();
                doc.Save(imgStream, options);
                // Insert the image stream into a temporary Document instance.
                Document temp = new Document();
                DocumentBuilder builder = new DocumentBuilder(temp);
                var img = builder.InsertImage(imgStream);
                // Resize the image as per your needs
                img.Width = 595;
                img.Height = 842;
                // Save the individual image to disk using ShapeRenderer class

                ShapeRenderer renderer = img.GetShapeRenderer();
                int position = blobName.LastIndexOf('.');
                blobName = blobName.Substring(0, position);
                fileName = CurrentDirPath + @"\thumbnail_" + blobName + ".png";
                renderer.Save(fileName, new ImageSaveOptions(Aspose.Words.SaveFormat.Png));
                return fileName;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set Aspose licenece
        /// </summary>
        /// <returns></returns>
        private static bool SetAsposeLicence()
        {
            try
            {
                Aspose.Words.License slidelicense = new Aspose.Words.License();
                slidelicense.SetLicense("Aspose.Total");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool CreateHTML(Stream fileStream, string foldername, string filepath)
        {
            //DeleteFolder(filepath);
            bool isSuccess = false;
            SetAsposeLicence();
            try
            {
                //DeleteFolder(filepath);
                var document = new Aspose.Words.Document(fileStream);
                fileStream.Close();
                document.Save(filepath, Aspose.Words.SaveFormat.HtmlFixed);
                isSuccess = true;
                string fileContent = System.IO.File.ReadAllText(filepath);
                fileContent = fileContent.Replace("href=\"", "href=\"/" + foldername + "/");
                fileContent = fileContent.Replace("src=\"", "src=\"/" + foldername + "/");
                fileContent = fileContent.Replace("xlink:href=\"/" + foldername + "/", "xlink:href=\"");
                fileContent = fileContent.Replace("href=\"/" + foldername + "/#", "href=\"#");
                File.WriteAllText(filepath, fileContent);
                //DeleteFile(filepath);
            }
            catch (Exception ex)
            {
                Logger.Error("AsponseHelper -CreateHTML ", ex);
            }
            return isSuccess;
        }

        public static byte[] CreateHTML(Stream fileStream, string filepath, string fileName = "", string foldername = "")
        {
            byte[] htmlFile = null;
            DeleteFolder(filepath);
            SetAsposeLicence();
            try
            {
                var document = new Aspose.Words.Document(fileStream);
                fileStream.Close();
                document.Save(filepath, Aspose.Words.SaveFormat.HtmlFixed);

                string fileContent = System.IO.File.ReadAllText(filepath);
                fileContent = fileContent.Replace($"href=\"{fileName}", $"href=\"/{foldername}/{fileName}");//for style.css
                fileContent = fileContent.Replace($"src=\"{fileName}", $"src=\"/{foldername}/{fileName}");// "src=\"/" + foldername + "/");//for images
                fileContent = fileContent.Replace("a href=\"http", "a target=\"_blank\" href=\"http");//for externally hosted links so that they always open in new tab

                File.WriteAllText(filepath, fileContent);
                using (var file = System.IO.File.OpenRead(filepath))
                {
                    htmlFile = file.ReadAllBytes();
                }
                //DeleteFile(filepath);
            }
            catch (Exception ex)
            {
                Logger.Error("AsponseHelper -CreateHTML ", ex);
            }
            return htmlFile;
        }

        public static byte[] CreateHTMLFromDOC(Stream docfileStream, string filepath)
        {
            byte[] htmlFile = null;
            SetAsposeLicence();
            try
            {
                var document = new Aspose.Words.Document(docfileStream);
                docfileStream.Close();
                document.Save(filepath, Aspose.Words.SaveFormat.HtmlFixed);
                using (var file = System.IO.File.OpenRead(filepath))
                {
                    htmlFile = file.ReadAllBytes();
                }
                DeleteFile(filepath);
                docfileStream.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("AsponseHelper -CreateHTML ", ex);
            }
            return htmlFile;
        }

        private static void DeleteFile(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            fi.Delete();
        }

        #endregion public Methods

        #region private Methods

        private static void DeleteFolder(string filename)
        {
            int NoofDays = -1;
            String dir = filename.Substring(0, filename.LastIndexOf(@"\"));

            string[] files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.CreationTimeUtc < DateTime.UtcNow.AddDays(NoofDays))
                    fi.Delete();
            }

            foreach (string d in Directory.GetDirectories(dir))
            {
                DirectoryInfo di = new DirectoryInfo(d);
                if (di.CreationTimeUtc < DateTime.UtcNow.AddDays(NoofDays))
                    di.Delete();
            }
        }

        #endregion private Methods
    }

    public static class StreamExtensions
    {
        public static byte[] ReadAllBytes(this Stream instream)
        {
            if (instream is MemoryStream)
                return ((MemoryStream)instream).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                instream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}