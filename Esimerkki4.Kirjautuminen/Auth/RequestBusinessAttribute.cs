using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Controllers.Dtos;
using Esimerkki4.Kirjautuminen.Models;
using Esimerkki4.Kirjautuminen.Mvc;
using Esimerkki4.Kirjautuminen.Services;
using Esimerkki4.Kirjautuminen.Stores;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Esimerkki4.Kirjautuminen.Auth
{
    public class RequestBusinessModelBinder : IModelBinder
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly BusinessAuthorize businessAuthorize;

        public RequestBusinessModelBinder(UserManager<ApplicationUser> userManager, BusinessAuthorize authService)
        {
            this.userManager = userManager;
            this.businessAuthorize = authService;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            bindingContext.Result = ModelBindingResult.Success(
                await businessAuthorize.RequestBusiness(bindingContext.HttpContext.User)
            );
        }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequestBusinessAttribute : Attribute, IBinderTypeProviderMetadata
    {
        public BindingSource BindingSource
        {
            get
            {
                return new BindingSource(
                    id: "RequestBusiness",
                    displayName: "RequestBusiness",
                    isGreedy: false,
                    isFromRequest: false);
            }
        }

        Type IBinderTypeProviderMetadata.BinderType
        {
            get
            {
                return typeof(RequestBusinessModelBinder);
            }
        }

    }
}