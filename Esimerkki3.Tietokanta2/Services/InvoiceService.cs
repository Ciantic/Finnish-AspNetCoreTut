using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki3.Tietokanta2.Db;
using Esimerkki3.Tietokanta2.Models;
using Esimerkki3.Tietokanta2.Stores;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki3.Tietokanta2.Services {
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

        public async Task<Invoice> Send(Invoice invoice) {
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