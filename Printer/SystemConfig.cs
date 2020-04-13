using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printer
{

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class SystemConfig
    {

        private SystemConfigPrinter printerField;

        private SystemConfigProduct[] productsField;

        /// <remarks/>
        public SystemConfigPrinter Printer
        {
            get
            {
                return this.printerField;
            }
            set
            {
                this.printerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Product", IsNullable = false)]
        public SystemConfigProduct[] Products
        {
            get
            {
                return this.productsField;
            }
            set
            {
                this.productsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SystemConfigPrinter
    {

        private string nameField;

        private string ipPortField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string IpPort
        {
            get
            {
                return this.ipPortField;
            }
            set
            {
                this.ipPortField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SystemConfigProduct
    {

        private string nameField;

        private SystemConfigProductBaseCords baseCordsField;

        private SystemConfigProductData[] datasField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public SystemConfigProductBaseCords BaseCords
        {
            get
            {
                return this.baseCordsField;
            }
            set
            {
                this.baseCordsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Data", IsNullable = false)]
        public SystemConfigProductData[] Datas
        {
            get
            {
                return this.datasField;
            }
            set
            {
                this.datasField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SystemConfigProductBaseCords
    {

        private string baseCord1Field;

        private string baseCord2Field;

        private string baseCord3Field;
        private string baseCord4Field;
        private string baseCord5Field;
        private string baseCord6Field;
        private string baseCord7Field;
        private string baseCord8Field;

        /// <remarks/>
        public string BaseCord1
        {
            get
            {
                return this.baseCord1Field;
            }
            set
            {
                this.baseCord1Field = value;
            }
        }

        /// <remarks/>
        public string BaseCord2
        {
            get
            {
                return this.baseCord2Field;
            }
            set
            {
                this.baseCord2Field = value;
            }
        }

        /// <remarks/>
        public string BaseCord3
        {
            get
            {
                return this.baseCord3Field;
            }
            set
            {
                this.baseCord3Field = value;
            }
        }
        public string BaseCord4
        {
            get
            {
                return this.baseCord4Field;
            }
            set
            {
                this.baseCord4Field = value;
            }
        }
        public string BaseCord5
        {
            get
            {
                return this.baseCord5Field;
            }
            set
            {
                this.baseCord5Field = value;
            }
        }
        public string BaseCord6
        {
            get
            {
                return this.baseCord6Field;
            }
            set
            {
                this.baseCord6Field = value;
            }
        }
        public string BaseCord7
        {
            get
            {
                return this.baseCord7Field;
            }
            set
            {
                this.baseCord7Field = value;
            }
        }
        public string BaseCord8
        {
            get
            {
                return this.baseCord8Field;
            }
            set
            {
                this.baseCord8Field = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SystemConfigProductData
    {

        private string noField;

        private string typeField;

        private string textField;

        private string posXField;

        private string posYField;

        private string angleField;
        private string TemplateNoField;

        /// <remarks/>
        public string No
        {
            get
            {
                return this.noField;
            }
            set
            {
                this.noField = value;
            }
        }

        /// <remarks/>
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        public string PosX
        {
            get
            {
                return this.posXField;
            }
            set
            {
                this.posXField = value;
            }
        }

        /// <remarks/>
        public string PosY
        {
            get
            {
                return this.posYField;
            }
            set
            {
                this.posYField = value;
            }
        }

        /// <remarks/>
        public string Angle
        {
            get
            {
                return this.angleField;
            }
            set
            {
                this.angleField = value;
            }
        }

        public string TemplateNo
        {
            get
            {
                return this.TemplateNoField;
            }
            set
            {
                this.TemplateNoField = value;
            }
        }

        public double b { get; set; }
        public double k { get; set; }
    }


}
