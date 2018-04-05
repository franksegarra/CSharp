using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System;
using NAPowerWebAPI.Models;

namespace NAPowerWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : ApiController
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IAccountRepository AccountItems { get; private set; }

        public AccountController(IAccountRepository accountItems)
        {
            AccountItems = accountItems;
        }

        [Route("api/account")]
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            IEnumerable<UserAccount> accts = null;
            try
            {
                accts = AccountItems.GetAll();
                if (accts == null)
                {
                    return NotFound();
                }
                return Ok(accts);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
        }

        [Route("api/account/{id}")]
        [HttpGet]
        public IHttpActionResult GetUserAccount(int id)
        {
            UserAccount acct = null;
            try
            {
                acct = AccountItems.GetUserAccount(id);
                if (acct == null)
                {
                    return NotFound();
                }
                return Ok(acct);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return InternalServerError(ex);
            }
        }

        [Route("api/account")]
        [HttpPost]
        public IHttpActionResult AddAccount([FromBody] UserAccount account)
        {
            if (account == null)
            {
                return BadRequest();
            }

            try
            {
                AccountItems.Add(account);
                return Ok(account);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return InternalServerError(ex);
            }
        }
        
        [Route("api/account/{id}")]
        [HttpDelete]
        public void Delete(int id)
        {
            try
            {
                AccountItems.Remove(id);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        [Route("api/account/{id}")]
        [HttpPut]
        public IHttpActionResult Update(int id, [FromBody] UserAccount account)
        {
            if (account == null || account.Id != id)
            {
                return BadRequest();
            }

            var item = AccountItems.GetUserAccount(id);
            if (item == null)
            {
                return NotFound();
            }

            UserAccount acct = null;
            try
            {
                AccountItems.Update(account);
                acct = AccountItems.GetUserAccount(account.Id);
                if (acct == null)
                {
                    return NotFound();
                }
                return Ok(acct);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return InternalServerError(ex);
            }
        }

    }
}
