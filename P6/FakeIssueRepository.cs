﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace P6
{
    class FakeIssueRepository : IIssueRepository
    {
        public const string EMPTY_TITLE_ERROR = "A Title is required.";
        public const string EMPTY_DISCOVERY_DATETIME_ERROR = "Must select a Discovery Date/Time.";
        public const string FUTURE_DISCOVERY_DATETIME_ERROR = "Issues can be from the future.";
        public const string EMPTY_DISCOVERER_ERROR = "A Discoverer is required.";
        public const string DUPLICATE_TITLE_ERROR = "Title is already in use.";
        private const string NO_ERROR = "";

        private static List<Issue> _Issues = new List<Issue>();
        public string Add(Issue issue)
        {
            string check = ValidateIssue(issue);
            if (check == NO_ERROR)
                _Issues.Add(issue);
            return check;
        }

        private string ValidateIssue(Issue issue)
        {
            if (string.IsNullOrEmpty(issue.Title))
                return EMPTY_TITLE_ERROR;
            else if (isDuplicate(issue.Title))
                return DUPLICATE_TITLE_ERROR;
            else if (issue.DiscoveryDate == DateTime.MinValue)
                return EMPTY_DISCOVERY_DATETIME_ERROR;
            else if (DateTime.Compare(DateTime.Now, issue.DiscoveryDate) < 0)
                return FUTURE_DISCOVERY_DATETIME_ERROR;
            else if (string.IsNullOrEmpty(issue.Discoverer))
                return EMPTY_DISCOVERER_ERROR;
            return NO_ERROR;
        }

        private bool isDuplicate(string title)
        {
            foreach (Issue issue in _Issues)
            {
                if (issue.Title == title)
                {
                    return true;
                }
            }
            return false;
        }
        public List<Issue> GetAll(int ProjectId)
        {
            List<Issue> ret = new List<Issue>();
            foreach (Issue issue in _Issues)
            {
                if (ProjectId == issue.ProjectId)
                    ret.Add(issue);
            }
            return ret;
        }

        public Issue GetIssueById(int Id)
        {
            Issue ret = null;
            foreach (Issue issue in _Issues)
            {
                if (issue.Id == Id)
                {
                    ret = issue;
                    break;
                }
            }
            return ret;
        }

        public List<string> GetIssuesByDiscoverer(int ProjectId)
        {
            List<string> ret = new List<string>();
            Dictionary<string, int> counter = new Dictionary<string, int>();
            foreach (Issue issue in _Issues)
            {
                if (ProjectId == issue.ProjectId)
                {
                    if (counter.ContainsKey(issue.Discoverer))
                    {
                        counter[issue.Discoverer] += 1;
                        continue;
                    }
                    counter[issue.Discoverer] = 1;
                }
            }
            foreach (KeyValuePair<string, int> count in counter)
            {
                ret.Add($"{count.Key}: {count.Value}");
            }
            return ret;
        }

        public List<string> GetIssuesByMonth(int ProjectId)
        {
            List<string> ret = new List<string>();
            var query = _Issues.Where(i => i.ProjectId == ProjectId)
                .GroupBy(issue => new { issue.DiscoveryDate.Year, issue.DiscoveryDate.Month })
                .Select(g => new
                {
                    Key = string.Format("{0} - {1}: ", g.Key.Year, g.Key.Year),
                    Amount = g.Count(),
                });
                
            foreach (var issue in query)
            {
                ret.Add(string.Format("{0}{1}", issue.Key, issue.Amount));
            }
            return ret;
        }

        public int GetTotalNumberOfIssues(int ProjectId)
        {
            return _Issues.Where(i => i.ProjectId == ProjectId).Count();
        }

        public string Modify(Issue issue)
        {
            string check = ValidateIssue(issue);
            if (check == NO_ERROR)
            {
                foreach (Issue old in _Issues)
                {
                    if (issue.Id == old.Id)
                    {
                        old.ProjectId = issue.ProjectId;
                        old.Title = issue.Title;
                        old.DiscoveryDate = issue.DiscoveryDate;
                        old.Discoverer = issue.Discoverer;
                        old.InitialDescription = issue.InitialDescription;
                        old.Component = issue.Component;
                        old.IssueStatusId = issue.IssueStatusId;
                        break;
                    }
                }
            }
            return check;
        }

        public bool Remove(Issue issue)
        {
            return _Issues.Remove(issue);
        }
    }
}