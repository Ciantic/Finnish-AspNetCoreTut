using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Db;
using Esimerkki4.Kirjautuminen.Models;
using Esimerkki4.Kirjautuminen.Stores;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki4.Kirjautuminen.Services {
    public class InvoiceService
    {
        private readonly InvoiceStore invoiceStore;
        private readonly NotificationSender notificationSender;

        public InvoiceService(InvoiceStore invoiceStore, NotificationSender notificationSender)
        {
            this.invoiceStore = invoiceStore;
            this.notificationSender = notificationSender;
        }

        public async Task Update(Invoice invoice) {
            await invoiceStore.Update(invoice);
        }

        public async Task<Invoice> Create(Invoice invoice) {
            return await invoiceStore.Create(invoice);
        }

        public async Task Remove(Invoice invoice) {
            await invoiceStore.Remove(invoice);
        }

        public class InvoiceSendException : Exception {
            public InvoiceSendException(string message) : base(message)
            {
                
            }
        }

        public async Task<Invoice> Send(Invoice invoice) {
            if (invoice.Client == null || invoice.Client.Email == "") {
                throw new InvoiceSendException("Client or client email is missing");
            }
            invoice.Sent = DateTime.UtcNow;
            await invoiceStore.Update(invoice);
            await notificationSender.SendInvoice(invoice);
            return invoice;
        }

        public async Task<Invoice> GetByBusiness(int businessId, int id) {
            return await invoiceStore.GetByBusiness(businessId, id);
        }

        public async Task<ICollection<Invoice>> ListLatestByBusiness(int businessId) {
            return await invoiceStore.ListLatestByBusiness(businessId);
        }
    }
}