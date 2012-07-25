using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    public class ORDER_INFO : DatabaseCTLS
    {
        /// <summary>
        /// Order Info Table or Ordering Business Cards
        /// Use this Class For Business / Data Logic Extensions Specific To Order_Info Table
        /// </summary>
        public ORDER_INFO()
        {
            ClassName = "Data.ORDER_INFO";
            _tableName = "ORDER_INFO";
            _PrimaryKey = "UNIQUE_ID";

            Initialize();
        }

        /// <summary>
        /// Business Card - Database Field
        /// </summary>
        public byte[] BUSINESS_CARD
        {
            get
            {
                return (byte[])contents[getName()] ?? new byte[0];
            }
            set
            {
                contents[getName()] = value;
            }
        }

        /// <summary>
        /// Company ID -  Database Field - Company Placing Order
        /// </summary>
        public String COMPANY_ID
        {
            get
            {
                return (String)contents[getName()] ?? "";
            }
            set
            {
                contents[getName()] = value;
            }
        }

        /// <summary>
        /// Agent ID - Database Field - Sales Agent Getting Credit for Sale
        /// </summary>
        public String AGENT_ID
        {
            get
            {
                return (String)contents[getName()] ?? "";
            }
            set
            {
                contents[getName()] = value;
            }
        }

        /// <summary>
        /// User Name - Database Field - User Responsible for Placing Order
        /// </summary>
        public String USER_NAME
        {
            get
            {
                return (String)contents[getName()] ?? "";
            }
            set
            {
                contents[getName()] = value;
            }
        }

        /// <summary>
        /// Order Quantity - Database Field - Number of Cards Ordered
        /// </summary>
        public String ORDER_QTY
        {
            get
            {
                return (String)contents[getName()] ?? "";
            }
            set
            {
                contents[getName()] = value;
            }
        }
    }
}
