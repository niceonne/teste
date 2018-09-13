using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocalBitcoins.API
{
    public class PayPalApi
    {
        private static APIContext GetApiContext()
        {
            // Authenticate with PayPal
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);
            return apiContext;
        }
        public Invoice SendInvoice(string email, string reference, string totalamount, string currency)
        {
            var apiContext = GetApiContext();
            var TemplateInvoice = InvoiceTemplate.Get(apiContext, "TEMP-7V015177YY181050W");
            var TemplateInfo = TemplateInvoice.template_data;

            var item = TemplateInfo.items;
            var invoice = new Invoice
            {
                merchant_info = TemplateInfo.merchant_info,
                billing_info = new List<BillingInfo>
                {
                    new BillingInfo
                    {
                        email = email
                    },

                },
                items = new List<InvoiceItem>
                {
                    new InvoiceItem{
                        name = "Digital Goods",
                        quantity = 1,
                        unit_price = new Currency
                        {
                        currency = currency, value = totalamount,
                        },
                        tax = new Tax
                        {
                            name ="Service fees",
                            percent = (float)5.85
                        },
                    },
                },
                logo_url = TemplateInfo.logo_url,
                terms = TemplateInfo.terms,
                reference = reference,
                note = TemplateInfo.note,
                template_id = TemplateInvoice.template_id
            };
            var createdInvoice = invoice.Create(GetApiContext());
            createdInvoice.Send(GetApiContext());
            return createdInvoice;
        }

        public Invoice GetInvoiceInfo(string id)
        {
            // ### Api Context
            // Pass in a `APIContext` object to authenticate 
            // the call and to send a unique request id 
            // (that ensures idempotency). The SDK generates
            // a request id if you do not pass one explicitly. 
            // See [Configuration.cs](/Source/Configuration.html) to know more about APIContext.
            var apiContext = GetApiContext();
            var invoiceId = id;

            var invoice = Invoice.Get(GetApiContext(), invoiceId);
            return invoice;
                // For more information, please visit [PayPal Developer REST API Reference](https://developer.paypal.com/docs/api/).
        }

        public Invoice CancelInvoice(string id)
        {
            var apiContext = GetApiContext();
            var invoiceId = id;
            var cancelNotification = new CancelNotification
            {
                subject = "Payment Canceled",
                note = "The payment has been canceled.",
                send_to_merchant = true,
                send_to_payer = true
            };
            var createdInvoice = GetInvoiceInfo(id);
            createdInvoice.Cancel(GetApiContext(), cancelNotification);
            return createdInvoice;
        }

        public InvoiceTemplates getInvoiceTemplates()
        {
            var apiContext = GetApiContext();
            var Invoices = InvoiceTemplate.GetAll(apiContext);
            return Invoices;
        }

    }

}
