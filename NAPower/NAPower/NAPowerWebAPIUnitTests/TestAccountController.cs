using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAPowerWebAPI.Controllers;
using NAPowerWebAPI.Models;
using System.Web.Http;
using System.Web.Http.Results;
using System.Linq;

namespace NAPowerWebAPIUnitTests
{
    [TestClass]
    public class TestAccountController : IDisposable
    {

        private IAccountRepository acctrepo = new AccountRepository();
        private AccountController controller;

        public TestAccountController()
        {
            controller = new AccountController(acctrepo);
        }

        [TestMethod]
        public void GetAll()
        {
            //Test that controller method returns same results as repository method
            //Call directly to repository
            IEnumerable<UserAccount> tstaccts = acctrepo.GetAll();
            List<UserAccount> lsttstaccts = (from a in tstaccts select a).ToList();

            //Call to controller
            IHttpActionResult result = controller.GetAll();
            var contentResult = result as OkNegotiatedContentResult<IEnumerable<UserAccount>>;
            IEnumerable<UserAccount> rsltAccts = contentResult.Content;
            List<UserAccount> lstrsltAccts = (from a in rsltAccts select a).ToList();

            CollectionAssert.AreEqual(lsttstaccts, lstrsltAccts);
        }

        [TestMethod]
        public void GetUserAccount()
        {
            //Test that controller method returns same results as repository method
            UserAccount tstacct = acctrepo.GetUserAccount(1);

            //Call to controller
            IHttpActionResult result = controller.GetUserAccount(1);
            var rsltAcct = result as OkNegotiatedContentResult<UserAccount>;
            UserAccount acct = rsltAcct.Content;

            Assert.AreEqual(tstacct, acct);
        }

        [TestMethod]
        public void AddAccount()
        {
            UserAccount newacct = new UserAccount { Id = 4, Address = "Main st", Email = "a.b@nyu.edu", Name = "Frank", Postal = "12345" };
            IHttpActionResult addresult = controller.AddAccount(newacct);

            //Could use the content of the actionresult but intentionally want to make a separate call
            IHttpActionResult result = controller.GetUserAccount(4);
            var rsltAcct = result as OkNegotiatedContentResult<UserAccount>;
            UserAccount acct = rsltAcct.Content;

            Assert.AreEqual(newacct, acct);
        }

        [TestMethod]
        public void Delete()
        {
            // Act
            controller.Delete(1);
            IHttpActionResult result = controller.GetUserAccount(1);
            
            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Update()
        {
            IHttpActionResult getresult = controller.GetUserAccount(1);
            var updtAcct = getresult as OkNegotiatedContentResult<UserAccount>;
            UserAccount updtacct = updtAcct.Content;
            updtacct.Postal = "12345";
            IHttpActionResult updtresult = controller.Update(1, updtacct);

            //Could use the content of the actionresult but intentionally want to make a separate call
            IHttpActionResult result = controller.GetUserAccount(1);
            var rsltAcct = result as OkNegotiatedContentResult<UserAccount>;
            UserAccount acct = rsltAcct.Content;

            Assert.AreEqual(updtacct, acct);
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
                controller.Dispose();
            }

            // Free any unmanaged objects here. 
            disposed = true;
        }

        #endregion IDisposable

    }
}
