using System;
using System.Collections.Generic;
using System.Xml;

namespace TDMtoTDSMigrator
{
    public class TableObject
    {
        private string _typeId;
        private string _categoryName;
        
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
            this._typeId = obj.GetTypeId();
            this._categoryName = obj.GetCategoryName();
        }


        public string GetTypeId(){
            return this._typeId;
        }
        public string GetCategoryName()
        {
            return this._categoryName;
        }
        public List<string[]> GetAttributes()
        {
            return attributes;
        }


        public string FindCategoryName(string typeId , XmlNode metaInfoTypes)
        {   
            //Find the category name of the object corresponding to its typeId
            for (int i =0; i < metaInfoTypes.ChildNodes.Count; i++)
            {
                if (metaInfoTypes.ChildNodes[i].Attributes?[0].Value == typeId)
                {
                    return metaInfoTypes.ChildNodes[i].Attributes?[1].Value;
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
        public void SetCategoryName(string typeId, XmlNode metaInfoTypes)
        {
            this._categoryName = FindCategoryName(typeId, metaInfoTypes);
        }
        public void SetTypeId(string typeId)
        {
            this._typeId = typeId;
        }


        public void AddAttribute(string attNumber, string attValue)
        {

            this.attributes.Add(new string[] { attNumber, attValue } );
        }
        




    }
}
