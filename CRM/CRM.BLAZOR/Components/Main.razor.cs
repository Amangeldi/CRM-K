﻿using Microsoft.AspNetCore.Components;
using CRM.BLAZOR.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Threading;
using CRM.BLL.DTO;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using CRM.BLAZOR.Models;
using Newtonsoft.Json;
using System.Text;
using static CRM.BLAZOR.Controllers.AccountController;
using System.Text.Json;
using Blazored.LocalStorage;
using CRM.BLAZOR.Services;
using CRM.DAL.Entities;
using System.ComponentModel.DataAnnotations;
using BlazorInputFile;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using CRM.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.BLAZOR.Components
{
    public class MainBase : ComponentBase, IDisposable
    {
        #region INJECTS
        [Inject]
        protected CRM.BLL.Interfaces.IUserRegistrationService UserRegistrationService { get; set; }
        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        protected CRM.BLL.Interfaces.ILogService LogService { get; set; }
        [Inject]
        protected CRM.BLL.Interfaces.ITempService TempService { get; set; }
        [Inject]
        IAuthService AuthService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        CRM.BLL.Interfaces.ICompanyService CompanyService { get; set; }
        [Inject]
        protected CRM.BLL.Interfaces.IMailFindService MailFindService { get; set; }
        [Inject]
        protected CRM.BLL.Interfaces.ICsvService CsvService { get; set; }
        [Inject]
        protected CRM.BLL.Interfaces.ILemlistIntegrationService LemlistIntegrationService { get; set; }
        [Inject]
        protected CRM.BLL.Interfaces.IHunterIntegrationService HunterIntegrationService { get; set; }
        [Inject]
        protected IServiceScopeFactory _ServiceScopeFactory { get; set; }
        #endregion
        #region VARIABLES
        /// companies div BEGIN
        protected CancellationTokenSource _cts = new CancellationTokenSource();
        protected AuthenticationState authState;
        protected ClaimsPrincipal user;
        public CompanyModel SelectedCompany;
        protected int SelectedId;

        protected string login;
        protected string password;
        protected string LeftStyle { get; set; } = @"'title-new-companies' 'content' 'content' 'content' 'content' 'content' 'content' 'content' 'content' 'content'
'title-qualified'
'title-not-qualified'";
        protected string NewCompanyDisplay { get; set; } = "block";
        protected string QualifiedDisplay { get; set; } = "none";
        protected string NotQualifiedDisplay { get; set; } = "none";
        protected string ExceptionLabelDisplay { get; set; } = "none";
        protected string UploadStatus { get; set; }
        protected IEnumerable<CompanyDTO> NewCompanies;
        protected IEnumerable<CompanyDTO> QualifiedCompanies;
        protected IEnumerable<CompanyDTO> NotQualifiedCompanies;
        /// companies div END
        /// company-information div BEGIN
        /// company-information div END
        /// controls BEGIN
        public bool IsDisabled { get; set; }
        public string SendLemmlistModalDisplay = "none";
        public string AddContactModalDisplay = "none";
        public string ImportContactsModalDisplay = "none";
        public string AddLemlistStatisticModalDisplay = "none";
        public string MessageModalDisplay = "none";
        public string MessageForModal = "";
        public string MessageForLoading = "";
        public string ActionMessage = "";
        public string ExceptionLabel = "";
        protected IEnumerable<ContactDTO> contacts;
        protected IEnumerable<CountryDTO> countries;
        protected List<int> checkedContacts;
        public bool isLoading;
        protected IEnumerable<Linkedin> Linkedins;
        protected IEnumerable<ContactDTO> SendForContacts;
        public AddLemlistStatistic AddLemlistStatistic;
        /// logs div BEGIN
        public IEnumerable<LogDTO> logs;
        public GetUserDTO currentUser;
        public List<Task> LogTasks;
        /// controls END
        /// logs div END
        #endregion
        #region BASE_METHODS
        protected override async Task OnInitializedAsync()
        {
            MessageForLoading = "Проверка прав доступа, ожидайте";
            isLoading = true;
            authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                TempService.CurrentUser = await UserRegistrationService.GetCurrent(user.Identity.Name);
            }
            NewCompany = new CompanyRegistrationDTO();
            AddLemlistStatistic = new AddLemlistStatistic();
            checkedContacts = new List<int>();
            LogTasks = new List<Task>();

            await TempService.UpdateCompanies();
            await RenderUpdate();
            isLoading = false;
            //await StartCountdown();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //TempService.UpdateCompanies();
            if (firstRender)
            {

            }
            //return base.OnAfterRenderAsync(firstRender);
        }
        public void Dispose()
        {

            _cts.Cancel();
        }
        #endregion
        #region OTHER_METHODS
        /// companies div BEGIN

        /// prospect-finder div BEGIN

        public CompanyRegistrationDTO NewCompany;

        /// prospect-finder div END

        public async Task RenderUpdate()
        {
            NewCompanies = TempService.NewCompanies;
            QualifiedCompanies = TempService.QualifiedCompanies;
            NotQualifiedCompanies = TempService.NotQualifiedCompanies;
            countries = TempService.Countries;
            SelectedId = TempService.GetSelectedId();
            if (SelectedId != 0)
            {
                SelectedCompany = TempService.CompanyModels.Where(p => p.Id == SelectedId).FirstOrDefault();
                await InvokeAsync(StateHasChanged);
                contacts = await TempService.GetCompanyContacts(SelectedId);
            }
            else
            {
                SelectedCompany = null;
            }
            if (TempService.CurrentUser != null)
            {
                currentUser = TempService.CurrentUser;
                var logTemp = TempService.logs;
                if (logTemp != null)
                {
                    logs = TempService.logs.Where(p => p.UserId == currentUser.Id);
                }
            }
            SelectedCompany = TempService.CompanyModels.Where(p => p.Id == SelectedId).FirstOrDefault();
            Linkedins = TempService.Linkedins;
            await InvokeAsync(StateHasChanged);
        }
        public async Task Login()
        {
            MessageForLoading = "Проверка прав доступа, ожидайте";
            isLoading = true;
            LoginModel loginModel = new LoginModel
            {
                Email = login,
                Password = password
            };
            var result = await AuthService.Login(loginModel);

            if (result.Successful)
            {
                await TempService.UpdateCompanies();
            }
            await RenderUpdate();
            await InvokeAsync(StateHasChanged);
            //NavigationManager.NavigateTo("/");
            await OnInitializedAsync();
            isLoading = false;
        }
        public async Task Logout()
        {
            MessageForLoading = "Проверка прав доступа, ожидайте";
            isLoading = true;
            await AuthService.Logout();
            TempService.SetId(0);
            await TempService.UpdateCompanies();
            await RenderUpdate();
            await InvokeAsync(StateHasChanged);
            //NavigationManager.NavigateTo("/");
            await OnInitializedAsync();
            isLoading = false;
        }
        public async Task SelectCompanyElement(int id)
        {
            TempService.SetId(id);
            SelectedId = id;
            SelectedCompany = TempService.CompanyModels.Where(p => p.Id == SelectedId).FirstOrDefault();
            //LogTasks.Add(AddLog(id));
            //Task.WhenAll(LogTasks.ToArray());
            await AddLog(id);
            await RenderUpdate();
            //Dispose();
        }

        public async Task AddLog(int CompanyId = 0, string WebSite = null, string LinkedinOfTradingName = null,
            QualifyCompanyModel qualifyCompany = null, int count = 0, string LinkedinOfUser = null, string ActionMesage = null)
        {
            LogDTO logDTO = new LogDTO();
            if (WebSite != null)
            {
                logDTO = new LogDTO
                {
                    Action = "Перешел на сайт " + WebSite,
                    UserId = TempService.CurrentUser.Id
                };
                //Thread.Sleep(3000);
            }
            else if (WebSite == null && CompanyId != 0)
            {
                logDTO = new LogDTO
                {
                    Action = "Просмотрел компанию " + TempService.AllCompanies.Where(p => p.Id == SelectedId).FirstOrDefault().TradingName,
                    UserId = TempService.CurrentUser.Id
                };
            }
            else if (LinkedinOfTradingName != null)
            {
                logDTO = new LogDTO
                {
                    Action = "Просмотрел аккаунт LinkedIn компании  " + LinkedinOfTradingName,
                    UserId = TempService.CurrentUser.Id
                };
            }
            else if (qualifyCompany != null)
            {
                if (qualifyCompany.IsQualify)
                {
                    logDTO = new LogDTO
                    {
                        Action = "Изменил статус компании " + qualifyCompany.CompanyTradingName + " на Квалифицированный",
                        UserId = TempService.CurrentUser.Id
                    };
                }
                else
                {
                    logDTO = new LogDTO
                    {
                        Action = "Изменил статус компании " + qualifyCompany.CompanyTradingName + " на НЕ квалифицированный",
                        UserId = TempService.CurrentUser.Id
                    };
                }
            }
            else if (count != 0)
            {
                logDTO = new LogDTO
                {
                    Action = "Добавил " + count + " контактов в lemmlist",
                    UserId = TempService.CurrentUser.Id
                };
            }
            else if (LinkedinOfUser != null)
            {
                logDTO = new LogDTO
                {
                    Action = "Просмотрел аккаунт LinkedIn пользователя  " + LinkedinOfUser,
                    UserId = TempService.CurrentUser.Id
                };
            }
            else if (ActionMesage != null)
            {
                logDTO = new LogDTO
                {
                    Action = ActionMesage,
                    UserId = TempService.CurrentUser.Id
                };
            }
            /*await Task.Run(async () => 
            {
                await LogService.AddLog(logDTO);
                await TempService.UpdateLogs();
            });*/
            using (var scope = _ServiceScopeFactory.CreateScope())
            {
                var _logService = scope.ServiceProvider.GetService<ILogService>();
                //var _tempService = scope.ServiceProvider.GetService<ITempService>();
                await _logService.AddLog(logDTO);
                await TempService.UpdateLogs();
            }
            await RenderUpdate();
        }
        /// companies div END
        /// controls div BEGIN
        public async Task SetQualify()
        {
            if (SelectedId != 0)
            {
                await CompanyService.SetQualified(SelectedId);
                var company = await CompanyService.GetCompany(SelectedId);
                QualifyCompanyModel qualified = new QualifyCompanyModel { IsQualify = true, CompanyTradingName = company.TradingName };
                await AddLog(qualifyCompany: qualified);
            }

            await TempService.UpdateCompanies();
            TempService.SetId(0);
            Pause();
            await RenderUpdate();
        }
        public async Task SetNotQualify()
        {
            if (SelectedId != 0)
            {
                await CompanyService.SetNotQualified(SelectedId);
                var company = await CompanyService.GetCompany(SelectedId);
                QualifyCompanyModel notQualified = new QualifyCompanyModel { IsQualify = false, CompanyTradingName = company.TradingName };
                await AddLog(qualifyCompany: notQualified);
            }
            await TempService.UpdateCompanies();
            TempService.SetId(0);
            Pause();
            await RenderUpdate();
        }
        async Task Pause()
        {
            IsDisabled = true;
            await InvokeAsync(StateHasChanged);
            Thread.Sleep(1200);
            IsDisabled = false;
            await InvokeAsync(StateHasChanged);
        }
        /// controls div END
        public async Task AddCompany()
        {
            if (NewCompany != null)
            {
                try
                {
                    Validator.ValidateObject(NewCompany, new ValidationContext(NewCompany));
                    await CompanyService.CreateCompany(NewCompany);
                    ActionMessage = "Добавил новую компанию";
                    await AddLog(ActionMesage: ActionMessage + ": " + NewCompany.TradingName);
                    await TempService.UpdateCompanies();
                    await Close();
                    await RenderUpdate();
                }
                catch (Exception ex)
                {
                    ExceptionLabel = ex.Message;
                    ExceptionLabelDisplay = "block";
                }
                await InvokeAsync(StateHasChanged);
            }
        }
        /// prospect-finder div BEGIN
        public async Task Check(int value)
        {
            if (checkedContacts.Contains(value))
            {
                checkedContacts.Remove(value);
            }
            else
            {
                checkedContacts.Add(value);
            }
            await RenderUpdate();
            await InvokeAsync(StateHasChanged);
        }
        public async Task Send()
        {
            SendForContacts = await MailFindService.FindContactsForId(checkedContacts.ToArray());
            SendLemmlistModalDisplay = "block";
            await RenderUpdate();
            await InvokeAsync(StateHasChanged);
        }
        public async Task SendLemlist()
        {
            var results = (await LemlistIntegrationService.AddLeadsInCampaign(SendForContacts.ToList())).ToList();
            int successResults = results.Where(p => p.Result == true).Count();
            int failResults = results.Where(p => p.Result == false).Count();
            AddLemlistStatistic = new AddLemlistStatistic
            {
                successCount = successResults,
                failedCount = failResults
            };
            await AddLog(count: successResults);
            await Close();
            await OpenModalForAddLemlistStatistic();
            checkedContacts.Clear();
            await RenderUpdate();
        }
        public async Task ClearCheckedContacts()
        {
            checkedContacts.Clear();
            await RenderUpdate();
        }
        public async Task FindHunter()
        {
            
            if (SelectedCompany != null && SelectedCompany.Website != null)
            {
                MessageForLoading = "Поиск контактов "+ SelectedCompany.CompanyLegalName + ", ожидайте";
                isLoading = true;
                List<Contact> FoundContacts = (await HunterIntegrationService.FindDomainContacts(SelectedCompany.Website)).ToList();
                MessageForModal = "Найдены " + FoundContacts.Count + " новых контактов";
                MessageModalDisplay = "block";
                await TempService.UpdateCompanies();
                if (FoundContacts.Count != 0)
                {
                    ActionMessage = "Нашел " + FoundContacts.Count + " контактов компании " + SelectedCompany.CompanyLegalName;
                    await AddLog(ActionMesage: ActionMessage);
                }
                await InvokeAsync(StateHasChanged);
            }
            await RenderUpdate();
            isLoading = false;
        }
        public async Task OpenModalForNewCompany()
        {
            AddContactModalDisplay = "block";
            await RenderUpdate();
            await InvokeAsync(StateHasChanged);
        }
        public async Task OpenModalForImportFile()
        {
            ImportContactsModalDisplay = "block";
            await RenderUpdate();
            await InvokeAsync(StateHasChanged);
        }
        public async Task OpenModalForAddLemlistStatistic()
        {
            AddLemlistStatisticModalDisplay = "block";
            await RenderUpdate();
            await InvokeAsync(StateHasChanged);
        }

        public async Task Close()
        {
            AddContactModalDisplay = "none";
            SendLemmlistModalDisplay = "none";
            ImportContactsModalDisplay = "none";
            ExceptionLabelDisplay = "none";
            AddLemlistStatisticModalDisplay = "none";
            MessageModalDisplay = "none";
            await RenderUpdate();
            await InvokeAsync(StateHasChanged);
        }
        public async Task HandleSelection(IFileListEntry[] files)
        {
            string EndDirectory = "wwwroot/files/";
            var file = files.FirstOrDefault();
            if (file != null)
            {
                // Just load into .NET memory to show it can be done
                // Alternatively it could be saved to disk, or parsed in memory, or similar
                /*var ms = new MemoryStream();
                await file.Data.CopyToAsync(ms);*/
                using (FileStream DestinationStream = File.Create(EndDirectory + "file.csv"))
                {
                    await file.Data.CopyToAsync(DestinationStream);
                }
                await CsvService.ImportCSV();
                UploadStatus = $"Finished loading {file.Size} bytes from {file.Name}";
            }
            await RenderUpdate();
        }
        /// prospect-finder div END
        #endregion
        #region DISPLAY
        public async Task OpenNewCompaniesDiv()
        {
            NewCompanyDisplay = "block";
            QualifiedDisplay = "none";
            NotQualifiedDisplay = "none";
            LeftStyle = @"'title-new-companies''content' 'content' 'content' 'content' 'content' 'content' 'content' 'content' 'content'
'title-qualified'
'title-not-qualified'";
            await RenderUpdate();
        }
        public async Task OpenQualifiedDiv()
        {
            NewCompanyDisplay = "none";
            QualifiedDisplay = "block";
            NotQualifiedDisplay = "none";
            LeftStyle = @"'title-new-companies'
'title-qualified''content' 'content' 'content' 'content' 'content' 'content' 'content' 'content' 'content'
'title-not-qualified'";
            await RenderUpdate();
        }
        public async Task OpenNotQualifiedDiv()
        {
            NewCompanyDisplay = "none";
            QualifiedDisplay = "none";
            NotQualifiedDisplay = "block";
            LeftStyle = @"'title-new-companies'
'title-qualified'
'title-not-qualified''content' 'content' 'content' 'content' 'content' 'content' 'content' 'content' 'content'";
            await RenderUpdate();
        }
        #endregion
    }
}