using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using log4net;

namespace NAPowerMVCApp.Models
{
    class UserAccountModel : IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private HttpClient _client;

        public UserAccountModel()
        {
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            _client = new HttpClient();
            _client.BaseAddress = new Uri(appSettings["BaseAddressDev"]);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public List<UserAccount> GetAll()
        {
            List<UserAccount> lst = new List<UserAccount>();

            try
            {
                HttpResponseMessage response = _client.GetAsync("api/account").Result;
                if (response.IsSuccessStatusCode)
                {
                    lst = response.Content.ReadAsAsync<List<UserAccount>>().Result;
                }
                else
                {
                    string msg = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw;
            }

            return lst;
        }

        public UserAccount GetUserAccount(int id)
        {
            UserAccount useraccount = new UserAccount();

            try
            {
                HttpResponseMessage response = _client.GetAsync("api/account/" + id.ToString()).Result;
                if (response.IsSuccessStatusCode)
                {
                    useraccount = response.Content.ReadAsAsync<UserAccount>().Result;
                }
                else
                {
                    string msg = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw;
            }

            return useraccount;
        }

        public void AddAccount(UserAccount useraccount)
        {
            try
            {
                HttpResponseMessage response = _client.PostAsJsonAsync("api/account/", useraccount).Result;
                if (!response.IsSuccessStatusCode)
                {
                    string msg = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw;
            }
        }

        public void DeleteAccount(int id)
        {
            try
            {
                HttpResponseMessage response = _client.DeleteAsync("api/account/" + id.ToString()).Result;
                if (!response.IsSuccessStatusCode)
                {
                    string msg = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw;
            }
        }

        public void UpdateAccount(UserAccount useraccount)
        {
            try
            {
                HttpResponseMessage response = _client.PutAsJsonAsync("api/account/" + useraccount.Id.ToString() , useraccount).Result;
                if (response.IsSuccessStatusCode)
                {
                    UserAccount acct = response.Content.ReadAsAsync<UserAccount>().Result;
                }
                else
                {
                    string msg = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw;
            }
        }

        #region IDisposable
        // Flag: Has Dispose already been called? 
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers. 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _client.Dispose();
            }

            // Free any unmanaged objects here. 
            disposed = true;
        }

        #endregion IDisposable
    }
}
