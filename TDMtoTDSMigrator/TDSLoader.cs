using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using TDMtoTDSMigrator;
using System.Diagnostics;

namespace TDMtoTDSMigrator
{
    
    public class TDSLoader
    {

        public static string Decompress(FileInfo fi)
        {
            // Get the stream of the source file. 
            using (FileStream inFile = fi.OpenRead())
            {
                // Get original file extension, for example "doc" from report.doc.gz.
                string curFile = fi.FullName;
                string origName = curFile.Remove(curFile.Length - fi.Extension.Length);

                //Create the decompressed file. 
                string pathOfOutput = origName + ".xml";
                using (FileStream outFile = File.Create(pathOfOutput))
                {
                    using (GZipStream Decompress = new GZipStream(inFile,
                            CompressionMode.Decompress))
                    {
                        //Copy the decompression stream into the output file.
                        byte[] buffer = new byte[4096];
                        int numRead;
                        while ((numRead = Decompress.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            outFile.Write(buffer, 0, numRead);
                        }

                    }
                }
                return pathOfOutput;
            }

        }
        public static List<TableObject> TransformXMLIntoObjectList(string xmlPath)
        {
            List<TableObject> Objects = XMLParser.ParseXmlIntoObjectList(xmlPath);
            Console.WriteLine(Objects.Count + " lines of data");
            return Objects;
        }



        public static Task<HttpResponseMessage> LoadIntoTDS(string xmlPath, List<TableObject> list, string repositoryName, string apiURL)
        {
            
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);

            XmlNode metaInfoAttributes = XMLParser.GetMetaInfoAttributes(XMLParser.GetParentNodeOfData(doc));
            XmlNode metaInfoType = XMLParser.GetMetaInfoTypes(XMLParser.GetParentNodeOfData(doc));

            Task<HttpResponseMessage> message = null;
            for (int i = 0; i < list.Count; i++)
            {
                message = HTTPRequest.PostObject(JSONConverter.JsonifyObjectForAPI(list[i], metaInfoAttributes),repositoryName, apiURL);
            }

            // returns the response of the last request
            return message;
        }
        public static Task<HttpResponseMessage> LoadIntoTDSWithFilter(string xmlPath, List<TableObject> list, string repositoryName, List<string> authorizedTypes, string apiURL)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);

            XmlNode metaInfoAttributes = XMLParser.GetMetaInfoAttributes(XMLParser.GetParentNodeOfData(doc));
            XmlNode metaInfoType = XMLParser.GetMetaInfoTypes(XMLParser.GetParentNodeOfData(doc));

            Task<HttpResponseMessage> message = null;
            for (int i = 0; i < list.Count; i++)
            {

                if (authorizedTypes.Contains(list[i].GetTypeName()))
                {
                    message = HTTPRequest.PostObject(JSONConverter.JsonifyObjectForAPI(list[i], metaInfoAttributes), repositoryName, apiURL);  
                }
            }

            // returns the response of the last request
            return message;


        }


        


        /*
         Unused methods
         public static string CreateJSONString(string filepath)
        {
            XmlDocument doc = new XmlDocument();
            string xmlPath = Decompress(new FileInfo(filepath));
            doc.Load(xmlPath);

            return JSONconverter.TransformObjectListIntoJSonEquivalent(TransformXMLIntoObjectList(xmlPath), doc);
        }



         
         
         */




    }
}
