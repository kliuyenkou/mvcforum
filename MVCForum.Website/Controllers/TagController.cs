﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCForum.Domain.Constants;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Website.ViewModels;

namespace MVCForum.Website.Controllers
{
    public class TagController : BaseController
    {
        private readonly ITopicTagService _topicTagService;

        public TagController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, ITopicTagService topicTagService)
            : base(loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService)
        {
            _topicTagService = topicTagService;
        }

        [ChildActionOnly]
        public PartialViewResult PopularTags()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var popularTags = _topicTagService.GetPopularTags(100);
                var viewModel = new PopularTagViewModel { PopularTags = popularTags };
                return PartialView(viewModel);
            }
        }

    }
}
