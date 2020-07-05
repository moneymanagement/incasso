using Abp.Application.Navigation;
using Abp.Localization;
using Incasso;
using Incasso.Authorization;

namespace Outsourcing.Web
{
    /// <summary>
    /// This class defines menus for the application.
    /// It uses ABP's menu system.
    /// When you add menu items here, they are automatically appear in angular application.
    /// See Views/Layout/_TopMenu.cshtml file to know how to render menu.
    /// </summary>
    public class incassoNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.Users,
                        L("Users"),
                        url: "Users",
                        //icon: "home",
                        requiresAuthentication: true
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Administrations,
                        L("Administrations"),
                        url: "Administrations"
                    //,
                    //icon: "business",
                    //requiredPermissionName: PermissionNames.Pages_Tenants
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Debtors,
                        L("Debtors"),
                        url: "Debtors"
                        //,
                        //icon: "people",
                        //requiredPermissionName: PermissionNames.Debtors
                    )
                ) 
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.UploadData,
                        L("UploadData"),
                        url: "UploadData"
                        //,
                        //icon: "info"
                    )
                    ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Actions,
                        L("Actions"),
                        url: "Actions"
                    //,
                    //icon: "info"
                    )

                );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, incassoConsts.LocalizationSourceName);
        }
    }
}
