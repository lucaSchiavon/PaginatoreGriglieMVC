﻿// Author:					Martijn Boland/Joe Audette
// Created:					2015-07-06
// Last Modified:			2018-06-16
//
// borrowed most code for building the list of links from Martijn Boland
// https://github.com/martijnboland/MvcPaging/blob/master/src/MvcPaging/Pager.cs MIT License

using System;
using System.Collections.Generic;

namespace cloudscribe.Web.Pagination
{
    public class PaginationLinkBuilder : IBuildPaginationLinks
    {
        /// <summary>
        /// the problem with paging is when you have too many pages
        /// to fit in the pager based on PaginationSettings.MaxPagerItems
        /// you need a strategy to leave out links for some pages
        /// while still being possible to navigate to any page
        /// this class implements one such strategy (implemented by  Martijn Boland)
        /// if you want to implement a different strategy you can plugin your own
        /// IBuildPaginationLinks implementation
        /// </summary>
        /// <param name="paginationSettings"></param>
        /// <param name="generateUrl"></param>
        /// <param name="firstPageText"></param>
        /// <param name="firstPageTitle"></param>
        /// <param name="previousPageText"></param>
        /// <param name="previousPageTitle"></param>
        /// <param name="nextPageText"></param>
        /// <param name="nextPageTitle"></param>
        /// <param name="lastPageText"></param>
        /// <param name="lastPageTitle"></param>
        /// <param name="spacerText"></param>
        /// <returns></returns>
        public List<PaginationLink> BuildPaginationLinks(
            PaginationSettings paginationSettings, 
            Func<long, string> generateUrl,
            string firstPageText,
            string firstPageTitle,
            string previousPageText,
            string previousPageTitle,
            string nextPageText,
            string nextPageTitle,
            string lastPageText,
            string lastPageTitle,
            string spacerText = "..."
            )
        {
            List<PaginationLink> paginationLinks = new List<PaginationLink>();

            long totalPages = (long)Math.Ceiling(paginationSettings.TotalItems / (double)paginationSettings.ItemsPerPage);

            // First page
            if (paginationSettings.ShowFirstLast)
            {
                var isActive = paginationSettings.CurrentPage > 1;
                if(isActive || !paginationSettings.SuppressInActiveFirstLast)
                paginationLinks.Add(
                    new PaginationLink
                    {
                        Active = isActive,
                        Text = firstPageText,
                        Title = firstPageTitle,
                        PageNumber = 1,
                        Url = generateUrl(1)
                    });
            }

            // Previous page
            if(!paginationSettings.RemoveNextPrevLinks)
            {
                if (paginationSettings.UseReverseIncrement)
                {
                    var isActive = paginationSettings.CurrentPage < totalPages;

                    //SuppressEmptyNextPrev
                    if (isActive)
                    {
                        paginationLinks.Add(
                            new PaginationLink
                            {
                                Active = true,
                                PageNumber = paginationSettings.CurrentPage + 1,
                                Text = previousPageText,
                                Title = previousPageTitle,
                                Url = generateUrl(paginationSettings.CurrentPage + 1)
                            });
                    }
                    else
                    {
                        if (!paginationSettings.SuppressEmptyNextPrev)
                        {
                            paginationLinks.Add(
                            new PaginationLink
                            {
                                Active = false,
                                Text = previousPageText,
                                PageNumber = totalPages
                            });
                        }

                    }


                }
                else
                {
                    var isActive = paginationSettings.CurrentPage > 1;
                    if (isActive)
                    {
                        paginationLinks.Add(new PaginationLink
                        {
                            Active = true,
                            Text = previousPageText,
                            Title = previousPageTitle,
                            PageNumber = paginationSettings.CurrentPage - 1,
                            Url = generateUrl(paginationSettings.CurrentPage - 1)
                        });
                    }
                    else
                    {
                        if (!paginationSettings.SuppressEmptyNextPrev)
                        {
                            paginationLinks.Add(new PaginationLink
                            {
                                Active = false,
                                Text = previousPageText,
                                PageNumber = 1,
                                Url = generateUrl(1)
                            });
                        }

                    }


                }
            }
            
            


            if (paginationSettings.ShowNumbered)
            {
                long start = 1;
                long end = totalPages;
                long nrOfPagesToDisplay = paginationSettings.MaxPagerItems;

                if (totalPages > nrOfPagesToDisplay)
                {
                    var middle = (long)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                    var below = (paginationSettings.CurrentPage - middle);
                    var above = (paginationSettings.CurrentPage + middle);

                    if (below < 2)
                    {
                        above = nrOfPagesToDisplay;
                        below = 1;
                    }
                    else if (above > (totalPages - 2))
                    {
                        above = totalPages;
                        below = (totalPages - nrOfPagesToDisplay + 1);
                    }

                    start = below;
                    end = above;
                }

                if (start > 1)
                {
                    paginationLinks.Add(new PaginationLink
                    {
                        Active = true,
                        PageNumber = 1,
                        IsCurrent = (paginationSettings.CurrentPage == 1 ? true : false),
                        Text = "1",
                        Url = generateUrl(1)
                    });

                    if (start > 3)
                    {
                        paginationLinks.Add(new PaginationLink
                        {
                            Active = true,
                            PageNumber = 2,
                            IsCurrent = (paginationSettings.CurrentPage == 2 ? true : false),
                            Text = "2",
                            Url = generateUrl(2)
                        });
                    }

                    if (start > 2)
                    {
                        paginationLinks.Add(new PaginationLink
                        {
                            Active = false,
                            Text = spacerText,
                            IsSpacer = true
                        });
                    }
                }

                for (var i = start; i <= end; i++)
                {
                    if (i == paginationSettings.CurrentPage || (paginationSettings.CurrentPage <= 0 && i == 1))
                    {
                        paginationLinks.Add(new PaginationLink
                        {
                            Active = true,
                            PageNumber = i,
                            IsCurrent = (paginationSettings.CurrentPage == i ? true : false),
                            Text = i.ToString(),
                            Url = generateUrl(i)
                        });
                    }
                    else
                    {
                        paginationLinks.Add(new PaginationLink
                        {
                            Active = true,
                            PageNumber = i,
                            Text = i.ToString(),
                            IsCurrent = (paginationSettings.CurrentPage == i ? true : false),
                            Url = generateUrl(i)
                        });
                    }
                }

                if (end < totalPages)
                {
                    if (end < totalPages - 1)
                    {
                        paginationLinks.Add(new PaginationLink
                        {
                            Active = false,
                            Text = spacerText,
                            IsSpacer = true
                        });
                    }
                    if (totalPages - 2 > end)
                    {
                        paginationLinks.Add(new PaginationLink
                        {
                            Active = true,
                            PageNumber = totalPages - 1,
                            Text = (totalPages - 1).ToString(),
                            IsCurrent = (paginationSettings.CurrentPage == (totalPages - 1) ? true : false),
                            Url = generateUrl(totalPages - 1)
                        });
                    }

                    paginationLinks.Add(new PaginationLink
                    {
                        Active = true,
                        PageNumber = totalPages,
                        Text = totalPages.ToString(),
                        IsCurrent = (paginationSettings.CurrentPage == totalPages ? true : false),
                        Url = generateUrl(totalPages)
                    });
                }

            }

            if(!paginationSettings.RemoveNextPrevLinks)
            {
                // Next page
                if (paginationSettings.UseReverseIncrement)
                {


                    //SuppressEmptyNextPrev
                    var isActive = paginationSettings.CurrentPage > 1;

                    if (isActive)
                    {
                        paginationLinks.Add(
                            new PaginationLink
                            {
                                Active = true,
                                Text = nextPageText,
                                Title = nextPageTitle,
                                PageNumber = paginationSettings.CurrentPage - 1,
                                Url = generateUrl(paginationSettings.CurrentPage - 1)
                            });
                    }
                    else
                    {
                        if (!paginationSettings.SuppressEmptyNextPrev)
                        {
                            paginationLinks.Add(
                                new PaginationLink
                                {
                                    Active = false,
                                    Text = nextPageText,
                                    PageNumber = 1,
                                    Url = generateUrl(1)
                                });
                        }
                    }

                    //paginationLinks.Add(
                    // paginationSettings.CurrentPage > 1 ? new PaginationLink
                    // {
                    //     Active = true,
                    //     Text = nextPageText,
                    //     Title = nextPageTitle,
                    //     PageNumber = paginationSettings.CurrentPage + 1,
                    //     Url = generateUrl(paginationSettings.CurrentPage + 1)
                    // } : new PaginationLink
                    // {
                    //     Active = false,
                    //     Text = nextPageText,
                    //     PageNumber = 1,
                    //     Url = generateUrl(1)
                    // });
                }
                else
                {
                    var isActive = paginationSettings.CurrentPage < totalPages;
                    if (isActive)
                    {
                        paginationLinks.Add(new PaginationLink
                        {
                            Active = true,
                            PageNumber = paginationSettings.CurrentPage + 1,
                            Text = nextPageText,
                            Title = nextPageTitle,
                            Url = generateUrl(paginationSettings.CurrentPage + 1)
                        });
                    }
                    else
                    {
                        if (!paginationSettings.SuppressEmptyNextPrev)
                        {
                            paginationLinks.Add(new PaginationLink
                            {
                                Active = false,
                                Text = nextPageText,
                                PageNumber = totalPages
                            });
                        }

                    }

                }

            }

            
                

            // Last page
            if (paginationSettings.ShowFirstLast)
            {
                var isActive = paginationSettings.CurrentPage < totalPages;
                if(isActive || !paginationSettings.SuppressInActiveFirstLast)
                paginationLinks.Add(new PaginationLink
                {
                    Active = isActive,
                    Text = lastPageText,
                    Title = lastPageTitle,
                    PageNumber = totalPages,
                    Url = generateUrl(totalPages)
                });
            }

            
            return paginationLinks;

        }

    }
}
