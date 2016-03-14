﻿using System;
using System.Net;
using System.Threading.Tasks;
using LtiLibrary.AspNet.Outcomes.v2;
using LtiLibrary.Core.Outcomes.v2;

namespace ConsumerCertification.Controllers
{
    public class LineItemsController : LineItemsControllerBase
    {
        // Simple "database" of lineitems for demonstration purposes
        public const string LineItemId = "ltilibrary-jdoe-2";
        private static LineItem _lineItem;

        public LineItemsController()
        {
            OnDeleteLineItem = context =>
            {
                if (string.IsNullOrEmpty(context.Id) || _lineItem == null || !_lineItem.Id.Equals(new Uri(context.Id)))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    _lineItem = null;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItem = context =>
            {
                if (string.IsNullOrEmpty(context.Id) || _lineItem == null || !_lineItem.Id.Equals(new Uri(context.Id)))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    context.LineItem = _lineItem;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItems = context =>
            {
                if (_lineItem == null ||
                    (!string.IsNullOrEmpty(context.ActivityId) &&
                     !context.ActivityId.Equals(_lineItem.AssignedActivity.ActivityId)))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    var id = new UriBuilder(Request.RequestUri) { Query = "firstPage" };
                    context.LineItemContainerPage = new LineItemContainerPage
                    {
                        Id = id.Uri,
                        LineItemContainer = new LineItemContainer
                        {
                            LineItemMembershipSubject = new LineItemMembershipSubject
                            {
                                ContextId = _lineItem.LineItemOf.ContextId,
                                LineItems = new[] { _lineItem }
                            }
                        }
                    };
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnPostLineItem = context =>
            {
                if (_lineItem != null)
                {
                    context.StatusCode = HttpStatusCode.BadRequest;
                    return Task.FromResult<object>(null);
                }

                context.LineItem.Id = new Uri(LineItemId, UriKind.Relative);
                context.LineItem.Results = new Uri(Request.RequestUri, "results");
                _lineItem = context.LineItem;
                context.StatusCode = HttpStatusCode.Created;
                return Task.FromResult<object>(null);
            };

            OnPutLineItem = context =>
            {
                if (context.LineItem == null || _lineItem == null || !_lineItem.Id.Equals(context.LineItem.Id))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    _lineItem = context.LineItem;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };
        }

    }
}
