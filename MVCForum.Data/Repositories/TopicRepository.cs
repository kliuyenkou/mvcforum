﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MVCForum.Data.Context;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.Interfaces;
using MVCForum.Domain.Interfaces.Repositories;

namespace MVCForum.Data.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly MVCForumContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"> </param>
        public TopicRepository(IMVCForumContext context)
        {
            _context = context as MVCForumContext;
        }

        public IList<Topic> GetAll()
        {
            return _context.Topic.ToList();
        }

        public IList<Topic> GetHighestViewedTopics(int amountToTake)
        {
            return _context.Topic
                .OrderByDescending(x => x.Views)
                .Take(amountToTake)
                .ToList();
        }

        public Topic Add(Topic topic)
        {
            topic.Id = Guid.NewGuid();
            _context.Topic.Add(topic);
            return topic;
        }

        public Topic Get(Guid id)
        {
            return _context.Topic.FirstOrDefault(x => x.Id == id);
        }

        public void Delete(Topic item)
        {
            _context.Topic.Remove(item);
        }

        public void Update(Topic item)
        {
            // Check there's not an object with same identifier already in context
            if (_context.Topic.Local.Select(x => x.Id == item.Id).Any())
            {
                throw new ApplicationException("Object already exists in context - you do not need to call Update. Save occurs on Commit");
            }
            _context.Entry(item).State = EntityState.Modified;  
        }

        public PagedList<Topic> GetRecentTopics(int pageIndex, int pageSize, int amountToTake)
        {

            // Get the topics using an efficient
            var results = _context.Topic                            
                            .OrderByDescending(x => x.CreateDate)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            // We might only want to display the top 100
            // but there might not be 100 topics
            var total = results.Count;
            if (total > amountToTake)
            {
                total = amountToTake;
            }

            // Return a paged list
            return new PagedList<Topic>(results, pageIndex, pageSize, total);
        }

        public IList<Topic> GetRecentRssTopics(int amountToTake)
        {
            // Get the topics using an efficient
            var results = _context.Topic
                .OrderByDescending(s => s.CreateDate)
                .Take(amountToTake)
                .ToList();
            return results;
        }

        public IList<Topic> GetTopicsByUser(Guid memberId)
        {
            // Get the topics using an efficient
            var results = _context.Topic
                .Where(x => x.User.Id == memberId)
                .ToList();
            return results;
        }

        public IList<Topic> GetAllTopicsByCategory(Guid categoryId)
        {
            var results = _context.Topic
                .Where(x => x.Category.Id == categoryId)
                .ToList();
            return results;
        }

        public PagedList<Topic> GetPagedTopicsByCategory(int pageIndex, int pageSize, int amountToTake, Guid categoryId)
        {

            // Get the topics using an efficient
            var results = _context.Topic
                                .Where(x => x.Category.Id == categoryId)
                                .OrderByDescending(x => x.IsSticky)
                                .ThenByDescending(x => x.CreateDate)
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            // We might only want to display the top 100
            // but there might not be 100 topics
            var total = results.Count;
            if (total > amountToTake)
            {
                total = amountToTake;
            }

            // Return a paged list
            return new PagedList<Topic>(results, pageIndex, pageSize, total);
        }

        public PagedList<Topic> GetPagedTopicsAll(int pageIndex, int pageSize, int amountToTake)
        {
            // Get the topics using an efficient
            var results = _context.Topic
                                .OrderByDescending(x => x.IsSticky)
                                .ThenByDescending(x => x.CreateDate)
                                .Take(pageSize)
                                .Skip((pageIndex - 1) * pageSize)
                                .ToList();

            // We might only want to display the top 100
            // but there might not be 100 topics
            var total = results.Count;
            if (total > amountToTake)
            {
                total = amountToTake;
            }

            // Return a paged list
            return new PagedList<Topic>(results, pageIndex, pageSize, total);
        }

        public IList<Topic> GetRssTopicsByCategory(int amountToTake, Guid categoryId)
        {
            return _context.Topic                               
                            .Where(x => x.Category.Id == categoryId)                                
                            .OrderByDescending(x => x.CreateDate)
                            .Take(amountToTake)
                            .ToList();
        }

        public PagedList<Topic> GetPagedTopicsByTag(int pageIndex, int pageSize, int amountToTake, string tag)
        {
            // Get the topics using an efficient
            var results = _context.Topic
                                .OrderByDescending(x => x.IsSticky)
                                .ThenByDescending(x => x.CreateDate)
                                .Where(e => e.Tags.Select(t => t.Tag).Contains(tag))
                                .Take(pageSize)
                                .Skip((pageIndex - 1) * pageSize)
                                .ToList();

            // We might only want to display the top 100
            // but there might not be 100 topics
            var total = results.Count;
            if (total > amountToTake)
            {
                total = amountToTake;
            }

            // Return a paged list
            return new PagedList<Topic>(results, pageIndex, pageSize, total);
        }

        public Topic GetTopicBySlug(string slug)
        {
            return _context.Topic.SingleOrDefault(x => x.Slug == slug);
        }

        public IList<Topic> GetTopicBySlugLike(string slug)
        {
            return _context.Topic
                .Where(x => x.Slug.Contains(slug))
                .ToList();
        }

        public int TopicCount()
        {
            return _context.Topic.Count();
        }

        /// <summary>
        /// Get all posts that are solutions, by user
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public IList<Topic> GetSolvedTopicsByMember(Guid memberId)
        {
            return _context.Topic
                .Where(x => x.User.Id == memberId)
                .Where(x => x.Posts.Select(p => p.IsSolution).Contains(true))
                .ToList();
        }
    }
}