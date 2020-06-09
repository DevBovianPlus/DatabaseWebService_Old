[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(DatabaseWebService.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(DatabaseWebService.App_Start.NinjectWebCommon), "Stop")]

namespace DatabaseWebService.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using DatabaseWebService.Domain.Abstract;
    using DatabaseWebService.Domain.Concrete;

    using Ninject;
    using Ninject.Web.Common;
    using DatabaseWebService.DomainDW.Abstract;
    using DatabaseWebService.DomainDW.Concrete;
    using DatabaseWebService.DomainOTP.Concrete;
    using DatabaseWebService.DomainOTP.Abstract;
    using DatabaseWebService.DomainPDO.Abstract;
    using DatabaseWebService.DomainPDO.Concrete;
    using DatabaseWebService.DomainNOZ.Concrete;
    using DatabaseWebService.DomainNOZ.Abstract;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);

                System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new LocalNinjectDependencyResolver(kernel);

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IUserRepository>().To<UserRepository>();
            kernel.Bind<IEmployeeRepository>().To<EmployeeRepository>();
            kernel.Bind<IEmployeeRankRepository>().To<EmployeeRankRepository>();
            kernel.Bind<IPostRepository>().To<PostRepository>();
            kernel.Bind<IClientRepository>().To<ClientRepository>();
            kernel.Bind<IEventRepository>().To<EventRepository>();
            kernel.Bind<ISystemMessageEventsRepository>().To<SystemMessageEventsRepository>();
            kernel.Bind<IChartsRepository>().To<ChartsRepository>();
            kernel.Bind<IFinancialControlRepository>().To<FinancialControlRepository>();
            kernel.Bind<ISystemEmailMessageRepository>().To<SystemEmailMessageRepository>();
            //DW
            kernel.Bind<ICashFlow_SkupnoRepository>().To<CashFlow_SkupnoRepository>();
            //Grafolit_OTP
            kernel.Bind<IUserOTPRepository>().To<UserOTPRepository>();
            kernel.Bind<IOrderRepository>().To<OrderRepository>();
            kernel.Bind<IRecallRepository>().To<RecallRepository>();
            kernel.Bind<IClientOTPRepository>().To<ClientOTPRepository>();
            kernel.Bind<IRouteRepository>().To<RouteRepository>();
            kernel.Bind<ITenderRepository>().To<TenderRepository>();
            kernel.Bind<IEmployeeOTPRepository>().To<EmployeeOTPRepository>();
            kernel.Bind<IMSSQLFunctionsRepository>().To<MSSQLFunctionsRepository>();
            kernel.Bind<ISystemMessageEventsRepository_OTP>().To<SystemMessageEventsRepository_OTP>();
            kernel.Bind<ISystemEmailMessageRepository_OTP>().To<SystemEmailMessageRepository_OTP>();
            kernel.Bind<IUtilityServiceRepository>().To<UtilityServiceRepository>();
            kernel.Bind<IDashboardRepository>().To<DashboardRepository>();

            //Grafolit_PDO
            kernel.Bind<IUserPDORepository>().To<UserPDORepository>();
            kernel.Bind<IInquiryRepository>().To<InquiryRepository>();
            kernel.Bind<IClientPDORepository>().To<ClientPDORepository>();
            kernel.Bind<IEmployeePDORepository>().To<EmployeePDORepository>();
            kernel.Bind<ISettingsRepository>().To<SettingsRepository>();
            kernel.Bind<IOrderPDORepository>().To<OrderPDORepository>();
            kernel.Bind<IMSSQLPDOFunctionRepository>().To<MSSQLPDOFunctionRepository>();
            kernel.Bind<IDashboardPDORepository>().To<DashboardPDORepository>();
            kernel.Bind<ISystemMessageEventsRepository_PDO>().To<SystemMessageEventsRepository_PDO>();
            kernel.Bind<ISystemEmailMessageRepository_PDO>().To<SystemEmailMessageRepository_PDO>();

            //Grafolit_NOZ
            kernel.Bind<IUserNOZRepository>().To<UserNOZRepository>();
            kernel.Bind<IClientNOZRepository>().To<ClientNOZRepository>();
            kernel.Bind<IEmployeeNOZRepository>().To<EmployeeNOZRepository>();
            kernel.Bind<ISettingsNOZRepository>().To<SettingsNOZRepository>();
            kernel.Bind<IMSSQLNOZFunctionRepository>().To<MSSQLNOZFunctionRepository>();
            kernel.Bind<IOptimalStockOrderRepository>().To<OptimalStockOrderRepository>();
            kernel.Bind<IDashboardNOZRepository>().To<DashboardNOZRepository>();
            kernel.Bind<ISystemEmailMessageRepository_NOZ>().To<SystemEmailMessageRepository_NOZ>();

        }
    }
}
