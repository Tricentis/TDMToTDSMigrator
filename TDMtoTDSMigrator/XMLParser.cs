using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

using TestDataContract.TestData;

namespace TDMtoTDSMigrator {
    public class XmlParser {
        #region Constructors and Destructors

        public static Dictionary<string, List<TestDataObject>> CreateDataList(TdmDataDocument tdmDataSheet) {
            Dictionary<string, RawDataObject> rawDataObjects = new Dictionary<string, RawDataObject>();
            foreach (StringAttribute stringAttribute in tdmDataSheet.StringAttributes) {
                try {
                    rawDataObjects.Add(stringAttribute.ObjectId, new RawDataObject());
                    rawDataObjects[stringAttribute.ObjectId].AddAttribute(stringAttribute.AttributeId, stringAttribute.AttributeValue);
                } catch (ArgumentException) {
                    rawDataObjects[stringAttribute.ObjectId].AddAttribute(stringAttribute.AttributeId, stringAttribute.AttributeValue);
                }
            }
            return ArrangeData(rawDataObjects, tdmDataSheet);
        }

        #endregion

        #region Public Methods and Operators

        public static string DecompressTddFileIntoXml(FileInfo fi) {
            using (FileStream inFile = fi.OpenRead()) {
                string curFile = fi.FullName;
                string origName = curFile.Remove(curFile.Length - fi.Extension.Length);

                string pathOfOutput = origName + ".xml";
                using (FileStream outFile = File.Create(pathOfOutput)) {
                    using (GZipStream decompress = new GZipStream(inFile, CompressionMode.Decompress)) {
                        byte[] buffer = new byte[4096];
                        int numRead;
                        while ((numRead = decompress.Read(buffer, 0, buffer.Length)) != 0) {
                            outFile.Write(buffer, 0, numRead);
                        }
                    }
                }
                return pathOfOutput;
            }
        }

        #endregion

        #region Methods

        private static Dictionary<string, List<TestDataObject>> ArrangeData(Dictionary<string, RawDataObject> rawDataObjects, TdmDataDocument tdmDataSheet) {
            Dictionary<string, List<TestDataObject>> sortedData = new Dictionary<string, List<TestDataObject>>();
            foreach (RawDataObject obj in rawDataObjects.Values) {
                obj.SetAllAttributes(tdmDataSheet);
                try {
                    sortedData.Add(obj.CategoryName, new List<TestDataObject>());
                    sortedData[obj.CategoryName].Add(obj.ConvertIntoTestDataObject());
                } catch (ArgumentException) {
                    sortedData[obj.CategoryName].Add(obj.ConvertIntoTestDataObject());
                }
            }
            return sortedData;
        }

        #endregion
    }
}