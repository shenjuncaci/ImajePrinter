using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printer
{

    // 注意: 生成的代码可能至少需要 .NET Framework 4.5 或 .NET Core/Standard 2.0。
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Products
    {

        private ProductsProduct productField;

        /// <remarks/>
        public ProductsProduct Product
        {
            get
            {
                return this.productField;
            }
            set
            {
                this.productField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ProductsProduct
    {

        private string nameField;

        private ProductsProductBaseCords baseCordsField;

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
        public ProductsProductBaseCords BaseCords
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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ProductsProductBaseCords
    {

        private string baseCord1Field;

        private string baseCord2Field;

        private string serialField;

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
        public string serial
        {
            get
            {
                return this.serialField;
            }
            set
            {
                this.serialField = value;
            }
        }
    }


}
