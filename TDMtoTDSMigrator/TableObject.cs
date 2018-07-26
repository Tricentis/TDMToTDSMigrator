using System;
using System.Collections.Generic;
using System.Xml;

namespace TDMtoTDSMigrator
{
    public class TableObject
    {
        private string typeId;
        private string typeName;
        
        private List<string[]> attributes;


        public TableObject()
        {
            this.attributes = new List<string[]>();
           
        }
        public TableObject(List<string[]> attributes)
        {
            this.attributes = attributes;
        }
        public TableObject(TableObject obj)
        {
            this.attributes = obj.GetAttributes();
            this.typeId = obj.GetTypeId();
            this.typeName = obj.GetTypeName();
        }


        public string GetTypeId(){
            return this.typeId;
        }
        public string GetTypeName()
        {
            return this.typeName;
        }
        public List<string[]> GetAttributes()
        {
            return attributes;
        }


        public string FindTypeName(string typeId , XmlNode metaInfoTypes)
        {//Find the type name of the object (category) corresponding to its typeId
            for (int i =0; i < metaInfoTypes.ChildNodes.Count; i++)
            {
                if (metaInfoTypes.ChildNodes[i].Attributes[0].Value == typeId)
                {
                    return metaInfoTypes.ChildNodes[i].Attributes[1].Value;
                }
            }
            return "Type not found";
        }      
        public string FindAttributeName(string attributeId, XmlNode metaInfoAttributes) 
        {
            for (int i = 0; i < metaInfoAttributes.ChildNodes.Count; i++)
            {
                if (metaInfoAttributes.ChildNodes[i].Attributes[0].Value == attributeId)
                {
                    return metaInfoAttributes.ChildNodes[i].Attributes[1].Value;
                }
            }
            return "Type not found";
        }
        public void SetAttributeNames(XmlNode metaInfoAttributes)
        {//finds the name of each attribute id that is contained in the list and replaces id string by name string.
            for(int i = 0; i < this.attributes.Count; i++)
            {
                this.attributes[i][0] = FindAttributeName(this.attributes[i][0],metaInfoAttributes);
            }
        }
        public void SetTypeName(string typeId, XmlNode metaInfoTypes)
        {
            this.typeName = FindTypeName(typeId, metaInfoTypes);
        }
        public void SetTypeId(string typeId)
        {
            this.typeId = typeId;
        }


        public void AddAttribute(string attNumber, string attValue)
        {

            this.attributes.Add(new string[] { attNumber, attValue } );
        }
        




    }
}
