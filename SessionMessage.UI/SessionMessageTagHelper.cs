using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SessionMessage.Core;

namespace SessionMessage.UI
{
    [HtmlTargetElement("sessionmessage")]
    public class SessionMessageTagHelper : TagHelper
    {
        IHostingEnvironment _hostingEnvironment;
        private IUrlHelperFactory _urlHelper;
        IActionContextAccessor _actionContextAccessor;
        private ISessionMessageManager _sessionMessageManager;
        //private string _imagePath = UrlHelper.GenerateContentUrl("~/Scripts", new HttpContextWrapper(HttpContext.Current))+"/Images/";
  //      private int _timeout = 5000;
		//private int _extendedTimeout = 0;
		//private Position _position = Position.TopRight;
		//private bool _newestOnTop = false;
		//private AnimitionEffect _showMethod = AnimitionEffect.FadeIn;
		//private AnimitionEffect _hideMethod = AnimitionEffect.FadeOut;
		//private bool _progressBar = false;
		//private bool _closeButton = false;
		//private AnimitionEffect _closeMethod = AnimitionEffect.FadeOut;
		//public FluentSessionMessage ImagePath(string path)
		//{
		//	_imagePath = path;
		//	return this;
		//}
        public SessionMessageTagHelper(IHostingEnvironment hostingEnvironment, IUrlHelperFactory urlHelper,IActionContextAccessor actionContextAccessor, ISessionMessageManager sessionMessageManager)
        {
            _hostingEnvironment = hostingEnvironment;
            _urlHelper = urlHelper;
            _actionContextAccessor = actionContextAccessor;
            _sessionMessageManager = sessionMessageManager;
        }
        [HtmlAttributeName("timeout")]
        public int Timeout { get; set; } = 5000;
        [HtmlAttributeName("extened-timeout")]
        public int ExtendedTimeout { get; set; } = 0;
        [HtmlAttributeName("display-position")]
        public Position DisplayPosition { get; set; } = Position.TopRight;
        [HtmlAttributeName("show-animition-effect")]
        public AnimitionEffect ShowAnimitionEffect { get; set; } = AnimitionEffect.FadeIn;
        [HtmlAttributeName("hide-animition-effect")]
        public AnimitionEffect HideAnimitionEffect { get; set; } = AnimitionEffect.FadeOut;
        [HtmlAttributeName("close-animition-effect")]
        public AnimitionEffect CloseAnimitionEffect { get; set; } = AnimitionEffect.FadeOut;
        [HtmlAttributeName("show-progressbar")]
        public bool Progressbar { get; set; } = false;
        [HtmlAttributeName("show-close-button")]
        public bool CloseButton { get; set; } = false;
        [HtmlAttributeName("newest-ontop")]
        public bool NewestOnTop { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            output.TagName = null;
            var builder = output.PostElement;
            builder.Clear();
            builder.AppendHtml(ToString());
        }
            /// <summary>
            /// Render all messages that have been set during execution of the controller action.
            /// </summary>
            /// <param name="htmlHelper"></param>
            /// <returns></returns>
            public string RenderHtml()
        {
            StringWriter writer;
            var messages = string.Empty;
            List<Core.SessionMessage> sessionMessages = _sessionMessageManager.GetMessage();
            TagBuilder messageWrapper = null, messageBoxBuilder = null, messageBoxStatusBar = null, messageBoxModalBuilder = null, messageBoxModal = null;
            messageWrapper = new TagBuilder("div");
            messageWrapper.Attributes.Add("id", "messagewrapper");
            messageWrapper.Attributes.Add("style", "display: none");
            if (sessionMessages != null && sessionMessages.Count > 0)
            {
                for (int i = 0; i < sessionMessages.Count; i++)
                {
                    var sessionMessage = sessionMessages[i];
                    switch (sessionMessage.Behavior)
                    {
                        case MessageBehaviors.Modal:
                            if (messageBoxModal == null)
                            {
                                messageBoxModal = new TagBuilder("div");
                                messageBoxModal.Attributes.Add("id", "messageboxmodal");
                            }
                            messageBoxModalBuilder = new TagBuilder("div");
                            //messageBoxModalBuilder.Attributes.Add("id", "messagebox" + i);
                            //messageBoxModalBuilder.Attributes.Add("behavior", ((int)sessionMessage.Behavior).ToString());
                            messageBoxModalBuilder.AddCssClass(String.Format("messagebox {0}", Enum.GetName(typeof(MessageType), sessionMessage.Type).ToLowerInvariant()));
                            if(!string.IsNullOrEmpty(sessionMessage.Key))
                                messageBoxModalBuilder.Attributes.Add("key", sessionMessage.Key);
                            messageBoxModalBuilder.InnerHtml.AppendHtml(sessionMessage.Message);
                            if (messageBoxModalBuilder != null)
                            {
                                writer = new StringWriter();
                                messageBoxModalBuilder.WriteTo(writer, HtmlEncoder.Default);
                                messageBoxModal.InnerHtml.AppendHtml(writer.ToString());
                            }
                            break;
                        case MessageBehaviors.StatusBar:
                        default:
                            if (messageBoxStatusBar == null)
                            {
                                messageBoxStatusBar = new TagBuilder("div");
                                messageBoxStatusBar.Attributes.Add("id", "messageboxstatusbar");
                            }
                            messageBoxBuilder = new TagBuilder("div");
                            //messageBoxBuilder.Attributes.Add("id", "messagebox" + i);
                            messageBoxBuilder.Attributes.Add("behavior", (sessionMessage.Behavior).ToString());
                            messageBoxBuilder.Attributes.Add("type", (sessionMessage.Type).ToString());
                            messageBoxBuilder.Attributes.Add("caption", sessionMessage.Caption);
                            messageBoxBuilder.AddCssClass(String.Format("messagebox {0}", Enum.GetName(typeof(MessageType), sessionMessage.Type).ToLowerInvariant()));
                            if(!string.IsNullOrEmpty(sessionMessage.Key))
                                messageBoxBuilder.Attributes.Add("key", sessionMessage.Key);
                            messageBoxBuilder.InnerHtml.AppendHtml(sessionMessage.Message);
                            writer = new StringWriter();
                            messageBoxBuilder.WriteTo(writer, HtmlEncoder.Default);
                            messageBoxStatusBar.InnerHtml.AppendHtml(writer.ToString());
                            break;
                    }
                }
                if (messageBoxStatusBar != null)
                {
                    writer = new StringWriter();
                    messageBoxStatusBar.WriteTo(writer, HtmlEncoder.Default);
                    messages = string.IsNullOrEmpty(writer.ToString()) ? null : writer.ToString();
                }
                if (messageBoxModal != null)
                {
                    writer = new StringWriter();
                    messageBoxModal.WriteTo(writer, HtmlEncoder.Default);
                    messages += string.IsNullOrEmpty(writer.ToString()) ? null : writer.ToString();
                }
				messageWrapper.InnerHtml.AppendHtml(messages);
            }
            writer = new StringWriter();
            messageWrapper.WriteTo(writer, HtmlEncoder.Default);
            messages = writer.ToString();
            return messages;
        }
		private string RenderScript()
		{
            bool hasCallback=false;
			StringBuilder scripts = new StringBuilder();
			StringBuilder options = new StringBuilder();
            StringBuilder callbackScripts = new StringBuilder();
            StringBuilder callbackWrapper = new StringBuilder();
            options.AppendFormat("$().ready(function () {{initSessionMessage('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');}});"
            , CloseButton.ToString().ToLower(), NewestOnTop.ToString().ToLower(), Progressbar.ToString().ToLower(),
			ConvertPosition(DisplayPosition), Timeout, ExtendedTimeout, ConvertAnimitioEffect(ShowAnimitionEffect),
			ConvertAnimitioEffect(HideAnimitionEffect), ConvertAnimitioEffect(CloseAnimitionEffect));
            scripts.AppendFormat("<script type='text/javascript'>{0}</script>",options.ToString());
            //var scriptPath=_urlHelper.GetUrlHelper(_actionContextAccessor.ActionContext).Content("~/lib");
            var scriptPath = _urlHelper.GetUrlHelper(_actionContextAccessor.ActionContext).Content("~");
            //scripts.AppendFormat("<script type='text/javascript' src='{0}/session-message/dist/js/sessionmessage{1}.js'></script>", scriptPath, _hostingEnvironment.IsDevelopment()?"":".min");
            scripts.AppendFormat("<script type='text/javascript' src='{0}/sessionmessage/Scripts/sessionmessage{1}.js'></script>", scriptPath, _hostingEnvironment.IsDevelopment() ? "" : ".min");
            callbackWrapper.AppendLine("function sessionMessageCloseCallback(event){");
            List<Core.SessionMessage> sessionMessages = _sessionMessageManager.GetMessage();
            if (sessionMessages != null && sessionMessages.Count > 0)
            {
                for (int i = 0; i < sessionMessages.Count; i++)
                {
                    var sessionMessage = sessionMessages[i];
                    switch (sessionMessage.Behavior)
                    {
                        case MessageBehaviors.Modal:
                            if (!string.IsNullOrWhiteSpace(sessionMessage.CloseCallBack))
                            {
                                hasCallback = true;
                                callbackScripts.AppendFormat("function smcc{0}(event){{{1}}}",i,sessionMessage.CloseCallBack);
                                callbackWrapper.AppendLine(string.Format("smcc{0}(event);", i));
                            }
                            break;
                    }
                }
            }
            if (hasCallback)
            {
                scripts.AppendLine("<script type='text/javascript'>");
                scripts.AppendLine(callbackScripts.ToString());
                scripts.AppendLine(callbackWrapper.ToString());
                callbackWrapper.Append("}");
                scripts.AppendLine("</script>");
            }
            return scripts.ToString();
		}
		private string RenderCss()
		{
			StringBuilder css=new StringBuilder();
            //var contentPath = _urlHelper.GetUrlHelper(_actionContextAccessor.ActionContext).Content("~/lib");
            var contentPath = _urlHelper.GetUrlHelper(_actionContextAccessor.ActionContext).Content("~");
            //css.AppendFormat("<link rel='stylesheet' href='{0}/session-message/dist/css/sessionmessage{1}.css' />", contentPath, _hostingEnvironment.IsDevelopment()?"" : ".min");
            css.AppendFormat("<link rel='stylesheet' href='{0}/sessionmessage/Content/sessionmessage{1}.css' />", contentPath, _hostingEnvironment.IsDevelopment() ? "" : ".min");
            return css.ToString();
		}
		public override string ToString()
		{
			var content = new StringBuilder();
			content.AppendLine(RenderCss());
			content.AppendLine(RenderHtml());
			content.AppendLine(RenderScript());
            _sessionMessageManager.Clear();
            return content.ToString();
		}
		public string ToHtmlString()
		{
			return ToString();
		}
		private string ConvertAnimitioEffect(AnimitionEffect effect)
		{
			return effect.ToString().Substring(0,1).ToLower() + effect.ToString().Substring(1);
		}
		private string ConvertPosition(Position position)
		{
			string result=null;
			switch(position)
			{
				case Position.TopCenter:
					result = "toast-top-center";
					break;
				case Position.TopFullWidth:
					result = "toast-top-full-width";
					break;
				case Position.TopLeft:
					result = "toast-top-left";
					break;
				case Position.TopRight:
					result = "toast-top-right";
					break;
				case Position.BottomCenter:
					result = "toast-bottom-center";
					break;
				case Position.BottomFullWidth:
					result = "toast-bottom-full-width";
					break;
				case Position.BottomLeft:
					result = "toast-bottom-left";
					break;
				case Position.BottomRight:
					result = "toast-bottom-right";
					break;
			}
			return result;
		}
    }
	public enum AnimitionEffect
	{
		FadeIn,
		FadeOut,
		SlideUp,
		SlideDown
	}
	public enum Position
	{
		TopCenter,
		TopFullWidth,
		TopLeft,
		TopRight,
		BottomCenter,
		BottomFullWidth,
		BottomLeft,
		BottomRight
	}
}