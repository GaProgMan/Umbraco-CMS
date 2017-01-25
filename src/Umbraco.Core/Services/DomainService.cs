using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Umbraco.Core.Services
{
    public class DomainService : RepositoryService, IDomainService
    {
        public DomainService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
        }

        public bool Exists(string domainName)
        {
            using (var uow = UowProvider.GetUnitOfWork())
            {
                var repo = RepositoryFactory.CreateDomainRepository(uow);
                var ret = repo.Exists(domainName);
                uow.Commit();
                return ret;
            } 
        }

        public Attempt<OperationStatus> Delete(IDomain domain)
        {
            var evtMsgs = EventMessagesFactory.Get();
            if (Deleting.IsRaisedEventCancelled(
                   new DeleteEventArgs<IDomain>(domain, evtMsgs),
                   this))
            {
                return OperationStatus.Cancelled(evtMsgs);
            }

            using (var uow = UowProvider.GetUnitOfWork())
            {
                var repository = RepositoryFactory.CreateDomainRepository(uow);
                repository.Delete(domain);
                uow.Commit();
            }

            var args = new DeleteEventArgs<IDomain>(domain, false, evtMsgs);
            Deleted.RaiseEvent(args, this);
            return OperationStatus.Success(evtMsgs);
        }

        public IDomain GetByName(string name)
        {
            using (var uow = UowProvider.GetUnitOfWork())
            {
                var repository = RepositoryFactory.CreateDomainRepository(uow);
                var ret = repository.GetByName(name);
                uow.Commit();
                return ret;
            }
        }

        public IDomain GetById(int id)
        {
            using (var uow = UowProvider.GetUnitOfWork())
            {
	            var repo = RepositoryFactory.CreateDomainRepository(uow);
	            var ret = repo.Get(id);
                uow.Commit();
                return ret;
            }
        }

        public IEnumerable<IDomain> GetAll(bool includeWildcards)
        {
            using (var uow = UowProvider.GetUnitOfWork())
            {
                var repo = RepositoryFactory.CreateDomainRepository(uow);
                var ret = repo.GetAll(includeWildcards);
                uow.Commit();
                return ret;
            }
        }

        public IEnumerable<IDomain> GetAssignedDomains(int contentId, bool includeWildcards)
        {
            using (var uow = UowProvider.GetUnitOfWork())
            {
                var repo = RepositoryFactory.CreateDomainRepository(uow);
                var ret = repo.GetAssignedDomains(contentId, includeWildcards);
                uow.Commit();
                return ret;
            }
        }

        public Attempt<OperationStatus> Save(IDomain domainEntity)
        {
            var evtMsgs = EventMessagesFactory.Get();
            if (Saving.IsRaisedEventCancelled(
                    new SaveEventArgs<IDomain>(domainEntity, evtMsgs),
                    this))
            {
                return OperationStatus.Cancelled(evtMsgs);
            }

            using (var uow = UowProvider.GetUnitOfWork())
            {
                var repository = RepositoryFactory.CreateDomainRepository(uow);
                repository.AddOrUpdate(domainEntity);
                uow.Commit();
            }

            Saved.RaiseEvent(new SaveEventArgs<IDomain>(domainEntity, false, evtMsgs), this);
            return OperationStatus.Success(evtMsgs);
        }

        #region Event Handlers
        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IDomainService, DeleteEventArgs<IDomain>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IDomainService, DeleteEventArgs<IDomain>> Deleted;
      
        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IDomainService, SaveEventArgs<IDomain>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IDomainService, SaveEventArgs<IDomain>> Saved;

      
        #endregion
    }
}