using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CassandraWebTest.Models
{
    public static class SocialNetworkingHelper
    {
        #region[Twitter helper Method]   
        private static MvcHtmlString Social_Twitter(this HtmlHelper htmlHelper,
                 String title, String url)
        {
            TagBuilder social_link = new TagBuilder("a");
            social_link.Attributes.Add("href", "https://twitter.com/share");
            social_link.Attributes.Add("class", "twitter-share-button");
            social_link.Attributes.Add("data-via", "MY-TWITTER-HANDLE");
            social_link.Attributes.Add("data-count", "horizontal");
            social_link.Attributes.Add("data-text", title);
            social_link.SetInnerText("Tweet");
            social_link.Attributes.Add("data-url", url);
            return MvcHtmlString.Create(social_link.ToString(TagRenderMode.Normal));

        }
        #endregion

        #region[Facebook helper Method]
        private static MvcHtmlString Social_Facebook(this HtmlHelper htmlHelper,
              String title, String url)
        {
            StringBuilder str = new StringBuilder();

            TagBuilder social_link = new TagBuilder("div");
            social_link.Attributes.Add("class", "fb-like");
            social_link.Attributes.Add("data-send", "false");
            social_link.Attributes.Add("data-layout", "button_count");
            social_link.Attributes.Add("data-show-faces", "false");
            social_link.Attributes.Add("data-font", "arial");
            social_link.Attributes.Add("data-href", url);

            str.Append(social_link.ToString(TagRenderMode.Normal));
            return MvcHtmlString.Create(str.ToString());
        }
        #endregion

        #region[Google plus helper Method]
        private static MvcHtmlString Social_GooglePlusOne(this HtmlHelper htmlHelper,
            String title, String url)
        {
            StringBuilder str = new StringBuilder();

            TagBuilder social_link = new TagBuilder("div");
            social_link.Attributes.Add("class", "g-plusone");
            social_link.Attributes.Add("data-size", "medium");
            //social_link.Attributes.Add("data-size", "small");
            social_link.Attributes.Add("data-href", url);
            return MvcHtmlString.Create(social_link.ToString(TagRenderMode.Normal));
        }
        #endregion

        public static MvcHtmlString SocialLinkButtons(this HtmlHelper htmlHelper,
           String title, String url)
        {
            StringBuilder str = new StringBuilder();
            TagBuilder ul = new TagBuilder("ul");
            ul.AddCssClass("social");
            ul.Attributes.Add("style", "list-style:none; width:100%;");

            /* -------- Facebook ------- */
            TagBuilder li1 = new TagBuilder("li");
            li1.InnerHtml = htmlHelper.Social_Facebook(title, url).ToHtmlString();
            li1.AddCssClass("social-facebook");

            /* -------- Twitter ------- */
            TagBuilder li2 = new TagBuilder("li");
            li2.InnerHtml = htmlHelper.Social_Twitter(title, url).ToHtmlString();
            li2.AddCssClass("social-twitter");

            /* -------- Google --------*/
            TagBuilder li3 = new TagBuilder("li");
            li3.InnerHtml = htmlHelper.Social_GooglePlusOne(title, url).ToHtmlString();
            li3.AddCssClass("social-google");

            ul.InnerHtml = String.Format("{0}{1}{2}", li1, li2, li3);
            str.Append(ul.ToString(TagRenderMode.Normal));
            return MvcHtmlString.Create(str.ToString());
        }
    }
}