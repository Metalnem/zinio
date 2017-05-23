using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Storage;
using ZinioReaderWin8.Model.Library;
using ZinioReaderWin8.Util;

namespace ZinioReaderWin8.Services
{
	// Token: 0x0200009A RID: 154
	internal class IssueDataHelper
	{
		// Token: 0x06000530 RID: 1328 RVA: 0x00013D2C File Offset: 0x00011F2C
		public static Issue ParsePackingListContent(string packingListContent, string localDir, bool isExcerpt = false)
		{
			XElement xElement = XElement.Parse(packingListContent);
			XElement xElement2 = xElement.Descendants("status").FirstOrDefault<XElement>();
			if (xElement2 != null && xElement2.Value.Equals("0"))
			{
				return null;
			}
			Issue result;
			try
			{
				result = IssueDataHelper.ParsePackingList(xElement, localDir, isExcerpt);
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x00013EB4 File Offset: 0x000120B4
		private static async Task<Issue> ParsePackingList(StorageFile file, string relIssuePath, bool isExcerpt = false)
		{
			byte[] encryptedContent = await Utils.ReadAllBytes(file);
			string text = Utils.DecryptString(encryptedContent);
			Issue result;
			if (text == null)
			{
				result = null;
			}
			else
			{
				XElement xmlTree = XElement.Parse(text);
				result = IssueDataHelper.ParsePackingList(xmlTree, relIssuePath, isExcerpt);
			}
			return result;
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x00013F24 File Offset: 0x00012124
		private static Issue ParsePackingList(XElement xmlTree, string localDir, bool isExcerpt = false)
		{
			Dictionary<string, Control> dictionary = IssueDataHelper.ParseControls(xmlTree, localDir);
			List<ReflowableAsset> reflowableAssets = IssueDataHelper.ParseReflowableAssts(xmlTree);
			Page[] pages = IssueDataHelper.ParsePages(xmlTree, localDir, isExcerpt, dictionary);
			string empty = string.Empty;
			string value = xmlTree.Descendants("issueAssetDir").First<XElement>().Value;
			if (xmlTree.Descendants("reflowAssetDir").Any<XElement>())
			{
				value = xmlTree.Descendants("reflowAssetDir").First<XElement>().Value;
			}
			Issue issue = new Issue(pages, null);
			issue.IssueDirectory = localDir;
			issue.PubName = xmlTree.Descendants("pubName").First<XElement>().Value;
			issue.PubId = Convert.ToInt32(xmlTree.Descendants("pubId").First<XElement>().Value);
			issue.IssueTitle = xmlTree.Descendants("issueTitle").First<XElement>().Value.ToUpper();
			issue.IssueId = xmlTree.Descendants("issueId").First<XElement>().Value;
			issue.NumberOfPages = Convert.ToInt32(xmlTree.Descendants("numberOfPages").First<XElement>().Value);
			issue.HostName = xmlTree.Descendants("hostName").First<XElement>().Value;
			issue.IssueAssetUri = xmlTree.Descendants("issueAssetDir").First<XElement>().Value;
			issue.ReflowAssetUri = value;
			issue.Folios = (from o in xmlTree.Descendants("page")
			select o.Attribute("folio").Value).ToArray<string>();
			issue.MediaControls = dictionary;
			issue.ReflowableAssets = reflowableAssets;
			issue.Binding = ((xmlTree.Descendants("binding").FirstOrDefault<XElement>() != null) ? xmlTree.Descendants("binding").First<XElement>().Value : string.Empty);
			issue.IssueSinglePdfAssetUri = empty;
			issue.IsSinglePagePdf = (empty != string.Empty);
			issue.IsExcerptIssue = isExcerpt;
			Issue issue2 = issue;
			XElement xElement = null;
			IEnumerable<XElement> source = xmlTree.Descendants("trackingCode");
			if (source.Count<XElement>() != 0)
			{
				xElement = source.First<XElement>();
			}
			if (xElement != null)
			{
				string value2 = xElement.Attribute("init").Value;
				string value3 = xElement.Value;
				SettingManager instance = SettingManager.Instance;
				issue2.PdfPassword = IssueDataHelper.DecryptPdfPassword(value2, value3, instance.DeviceId, instance.InstallationUUID);
			}
			else
			{
				XElement xElement2 = xmlTree.Descendants("pdfPassword").First<XElement>();
				if (xElement2 != null)
				{
					issue2.PdfPassword = xElement2.Value;
				}
			}
			return issue2;
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x000146C4 File Offset: 0x000128C4
		private static Page[] ParsePages(XElement xmlTree, string issueDirectory, bool isExcerpt, Dictionary<string, Control> controlsDictionary)
		{
			if (xmlTree == null)
			{
				return new Page[0];
			}
			List<Page> list = (from p in xmlTree.Descendants("page")
			let areaCollection = 
				from d in p.Elements("area")
				select new Area
				{
					Type = ((d.Attribute("type") == null) ? AreaType.Unknown : Area.ConvertToAreaType(d.Attribute("type").Value)),
					Coords = ((d.Attribute("coords") == null) ? new Windows.Foundation.Rect(0.0, 0.0, 0.0, 0.0) : Area.ConvertToCoordsRect(d.Attribute("coords").Value)),
					Href = ((d.Attribute("href") == null) ? string.Empty : d.Attribute("href").Value),
					OfflineHref = ((d.Attribute("offlineHref") == null) ? string.Empty : d.Attribute("offlineHref").Value),
					IssueDirectory = issueDirectory,
					OnPageLoadEffect = ((d.Attribute("onPageLoadEffect") == null) ? PageLoadEffectType.None : Area.ConvertToPageLoadEffectType(d.Attribute("onPageLoadEffect").Value)),
					Control = ((d.Attribute("control") == null) ? null : controlsDictionary[d.Attribute("control").Value]),
					Img = ((d.Attribute("img") == null) ? string.Empty : d.Attribute("img").Value)
				}
			select new Page
			{
				Bookmark = ((p.Attribute("bookmark") == null) ? string.Empty : p.Attribute("bookmark").Value),
				Folio = p.Attribute("folio").Value,
				Media = (p.Attribute("media").Value != "0"),
				Number = Convert.ToInt32(p.Attribute("number").Value),
				Type = p.Attribute("type").Value,
				Areas = areaCollection.ToObservableCollection<Area>(),
				ImageObjects = new List<ImageObject>(),
				ImageHotspots = new List<ImageHotspot>(),
				ControlObjects = new List<ControlObject>(),
				IssueDirectory = issueDirectory,
				IsExcerptPage = isExcerpt,
				Width = ((p.Attribute("width") == null) ? -1 : Convert.ToInt32(p.Attribute("width").Value)),
				Height = ((p.Attribute("height") == null) ? -1 : Convert.ToInt32(p.Attribute("height").Value))
			}).ToList<Page>();
			return list.ToArray();
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x00014768 File Offset: 0x00012968
		private static List<Action> ParseActions(XElement objectXml)
		{
			return (from o in objectXml.Elements("action")
			select new Action
			{
				Type = Action.ConvertToActionMessagType(o.Attribute("type").Value)
			}).ToList<Action>();
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x00016A78 File Offset: 0x00014C78
		private static Dictionary<string, Control> ParseControls(XElement xmlTree, string issueDirectory)
		{
			IEnumerable<XElement> source = from p in xmlTree.Descendants("control")
			select p;
			Dictionary<string, Control> dictionary = new Dictionary<string, Control>();
			try
			{
				List<AudioButtonControl> list = (from p in source
				let type = p.Attribute("type")
				where type != null && type.Value == "audio-button"
				select <>h__TransparentIdentifier14).Select(delegate(<>h__TransparentIdentifier14)
				{
					AudioButtonControl audioButtonControl = new AudioButtonControl();
					audioButtonControl.Key = <>h__TransparentIdentifier14.p.Attribute("key").Value;
					audioButtonControl.Type = ControlType.Audio_Button;
					audioButtonControl.PlayButtonHref = (from d in <>h__TransparentIdentifier14.p.Elements("asset")
					where d.Attribute("key").Value.Equals("play-button")
					select d.Attribute("href").Value).FirstOrDefault<string>();
					audioButtonControl.StopButtonHref = (from d in <>h__TransparentIdentifier14.p.Elements("asset")
					where d.Attribute("key").Value.Equals("stop-button")
					select d.Attribute("href").Value).FirstOrDefault<string>();
					audioButtonControl.IssueDirectory = issueDirectory;
					return audioButtonControl;
				}).ToList<AudioButtonControl>();
				foreach (AudioButtonControl current in list)
				{
					dictionary.Add(current.Key, current);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				List<ButtonControl> list2 = (from p in source
				where p.Attribute("type").Value == "button"
				select p).Select(delegate(XElement p)
				{
					ButtonControl buttonControl = new ButtonControl();
					buttonControl.Key = p.Attribute("key").Value;
					buttonControl.Type = ControlType.Button;
					buttonControl.NormalHref = (from d in p.Elements("asset")
					where d.Attribute("key").Value.Equals("normal")
					select d.Attribute("href").Value).FirstOrDefault<string>();
					buttonControl.HighlightedHref = (from d in p.Elements("asset")
					where d.Attribute("key").Value.Equals("highlighted")
					select d.Attribute("href").Value).FirstOrDefault<string>();
					buttonControl.IssueDirectory = issueDirectory;
					return buttonControl;
				}).ToList<ButtonControl>();
				foreach (ButtonControl current2 in list2)
				{
					dictionary.Add(current2.Key, current2);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				List<InsetWebBrowserControl> list3 = (from p in source
				where p.Attribute("type").Value == "inset-html"
				let previewImageHref = (from d in p.Elements("asset")
				where d.Attribute("key").Value.Equals("preview-image")
				select d.Attribute("href").Value).FirstOrDefault<string>()
				let closeImagHref = (from d in p.Elements("asset")
				where d.Attribute("key").Value.Equals("close-image")
				select d.Attribute("href").Value).FirstOrDefault<string>()
				let backgroundImagHref = (from d in p.Elements("asset")
				where d.Attribute("key").Value.Equals("background-image")
				select d.Attribute("href").Value).FirstOrDefault<string>()
				let curtainColor = (from d in p.Elements("param")
				where d.Attribute("key").Value.Equals("curtain-color")
				select d.Attribute("value").Value).FirstOrDefault<string>()
				let insetRect = (from d in p.Elements("param")
				where d.Attribute("key").Value.Equals("inset-rect")
				select d.Attribute("value").Value).FirstOrDefault<string>()
				let backgroundStretchCap = (from d in p.Elements("param")
				where d.Attribute("key").Value.Equals("background-stretch-cap")
				select d.Attribute("value").Value).FirstOrDefault<string>()
				let contentPadding = (from d in p.Elements("param")
				where d.Attribute("key").Value.Equals("content-padding")
				select d.Attribute("value").Value).FirstOrDefault<string>()
				let closeCenter = (from d in p.Elements("param")
				where d.Attribute("key").Value.Equals("close-center")
				select d.Attribute("value").Value).FirstOrDefault<string>()
				select new InsetWebBrowserControl
				{
					Key = p.Attribute("key").Value,
					Type = ControlType.Inset_Web_Browser,
					PreviewImageHref = previewImageHref,
					CloseImageHref = closeImagHref,
					BackgroundImageHref = backgroundImagHref,
					CurtainColor = InsetWebBrowserControl.ConvertToCurtainColor(curtainColor),
					InsetRect = InsetWebBrowserControl.ConvertToInsetRect(insetRect),
					BackgroundStretchCap = InsetWebBrowserControl.ConvertToBackgroundStretchCap(backgroundStretchCap),
					ContentPadding = InsetWebBrowserControl.ConvertToContentPadding(contentPadding),
					CloseCenter = InsetWebBrowserControl.ConvertToCloseCenter(closeCenter),
					IssueDirectory = issueDirectory
				}).ToList<InsetWebBrowserControl>();
				foreach (InsetWebBrowserControl current3 in list3)
				{
					dictionary.Add(current3.Key, current3);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				List<ImageScrollerControl> list4 = (from p in source
				where p.Attribute("type").Value == "image-scroller"
				let hideHorizontalIndicator = (from d in p.Elements("param")
				where d.Attribute("key").Value.Equals("hide-horizontal-indicator")
				select d.Attribute("value").Value).FirstOrDefault<string>()
				let hideVerticalIndicator = (from d in p.Elements("param")
				where d.Attribute("key").Value.Equals("hide-vertical-indicator")
				select d.Attribute("value").Value).FirstOrDefault<string>()
				let pagingEnabled = (from d in p.Elements("param")
				where d.Attribute("key").Value.Equals("paging-enabled")
				select d.Attribute("value").Value).FirstOrDefault<string>()
				let indicatorStyle = (from d in p.Elements("param")
				where d.Attribute("key").Value.Equals("indicator-style")
				select d.Attribute("value").Value).FirstOrDefault<string>()
				select new ImageScrollerControl
				{
					Key = p.Attribute("key").Value,
					Type = ControlType.Image_Scroller,
					HideHorizontalIndicator = Control.ConvertToBoolean(hideHorizontalIndicator),
					HideVerticalIndicator = Control.ConvertToBoolean(hideVerticalIndicator),
					PagingEnabled = Control.ConvertToBoolean(pagingEnabled),
					IndicatorStyle = ImageScrollerControl.ConvertToIndicatorStyle(indicatorStyle),
					IssueDirectory = issueDirectory
				}).ToList<ImageScrollerControl>();
				foreach (ImageScrollerControl current4 in list4)
				{
					dictionary.Add(current4.Key, current4);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				List<VideoPlayerControl> list5 = (from p in source
				where p.Attribute("type").Value == "video"
				let autoStart = (from d in p.Elements("param")
				where d.Attribute("key").Value.Equals("auto-start")
				select d.Attribute("value").Value).FirstOrDefault<string>()
				let loop = (from d in p.Elements("param")
				where d.Attribute("key").Value.Equals("loop")
				select d.Attribute("value").Value).FirstOrDefault<string>()
				let hideControls = (from d in p.Elements("param")
				where d.Attribute("key").Value.Equals("hide-controls")
				select d.Attribute("value").Value).FirstOrDefault<string>()
				select new VideoPlayerControl
				{
					Key = p.Attribute("key").Value,
					Type = ControlType.Video_Player,
					AutoStart = Control.ConvertToBoolean(autoStart),
					Loop = Control.ConvertToBoolean(loop),
					HideControls = Control.ConvertToBoolean(hideControls),
					IssueDirectory = issueDirectory
				}).ToList<VideoPlayerControl>();
				foreach (VideoPlayerControl current5 in list5)
				{
					dictionary.Add(current5.Key, current5);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				List<WebBrowserControl> list6 = (from p in source
				where p.Attribute("type").Value == "html"
				select new WebBrowserControl
				{
					Key = p.Attribute("key").Value,
					Type = ControlType.Web_Browser,
					IssueDirectory = issueDirectory
				}).ToList<WebBrowserControl>();
				foreach (WebBrowserControl current6 in list6)
				{
					dictionary.Add(current6.Key, current6);
				}
			}
			catch (Exception)
			{
			}
			return dictionary;
		}

		// Token: 0x06000536 RID: 1334 RVA: 0x000171EC File Offset: 0x000153EC
		private static List<ReflowableAsset> ParseReflowableAssts(XElement xmlTree)
		{
			XElement xElement = (from p in xmlTree.Descendants("articles")
			select p).FirstOrDefault<XElement>();
			List<ReflowableAsset> list = new List<ReflowableAsset>();
			if (xElement != null)
			{
				List<ReflowableAsset> collection = (from d in xElement.Descendants("toc")
				select new ReflowableAsset
				{
					Type = ReflowableAssetType.Toc,
					Url = d.Attribute("name").Value
				}).ToList<ReflowableAsset>();
				list.AddRange(collection);
				List<ReflowableAsset> collection2 = (from d in xElement.Descendants("javascript")
				select new ReflowableAsset
				{
					Type = ReflowableAssetType.Javascript,
					Url = d.Attribute("name").Value
				}).ToList<ReflowableAsset>();
				list.AddRange(collection2);
				List<ReflowableAsset> collection3 = (from d in xElement.Descendants("article")
				select new ReflowableAsset
				{
					Type = ReflowableAssetType.Article,
					Url = d.Attribute("name").Value
				}).ToList<ReflowableAsset>();
				list.AddRange(collection3);
			}
			return list;
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x00017308 File Offset: 0x00015508
		public static void LoadTocArticles(Issue issue)
		{
			if (issue.ReflowableAssets == null)
			{
				return;
			}
			ReflowableAsset reflowableAsset = issue.ReflowableAssets.FirstOrDefault((ReflowableAsset asset) => asset.Type == ReflowableAssetType.Toc);
			if (reflowableAsset == null)
			{
				return;
			}
			string uri;
			if (issue.IsExcerptIssue)
			{
				uri = string.Concat(new string[]
				{
					SettingManager.Instance.ExcerptsFolder.Path,
					"\\",
					issue.IssueDirectory,
					"\\",
					reflowableAsset.Url
				});
			}
			else
			{
				uri = string.Concat(new string[]
				{
					SettingManager.Instance.CurrentUserDirectory.Path,
					"\\",
					issue.IssueDirectory,
					"\\",
					reflowableAsset.Url
				});
			}
			try
			{
				XElement xElement = XElement.Load(uri);
				XNamespace xNamespace = "http://www.w3.org/1999/xhtml";
				if (xElement != null)
				{
					XName name = "section";
					XName name2 = "article";
					XName name3 = "blurb";
					if (xElement.GetDefaultNamespace() == xNamespace)
					{
						name = xNamespace + "section";
						name2 = xNamespace + "article";
						name3 = xNamespace + "blurb";
					}
					IEnumerable<XElement> enumerable = xElement.Descendants(name);
					int num = 1;
					Dictionary<int, List<TextArticle>> dictionary = new Dictionary<int, List<TextArticle>>();
					List<ArticleSection> list = new List<ArticleSection>();
					foreach (XElement current in enumerable)
					{
						ArticleSection articleSection = new ArticleSection();
						string sectionTitle = (current.Attribute("title") == null) ? string.Format("Other {0}", new object[]
						{
							num++
						}) : current.Attribute("title").Value;
						articleSection.SectionTitle = sectionTitle;
						IEnumerable<XElement> enumerable2 = current.Descendants(name2);
						foreach (XElement current2 in enumerable2)
						{
							string folio = (string)current2.Attribute("folio");
							int num2;
							if (Utils.LookupIssuePageNum(issue.Pages, folio, out num2))
							{
								TextArticle item = new TextArticle
								{
									FileName = (string)current2.Attribute("name"),
									Blurb = current2.Descendants(name3).First<XElement>().Value,
									PageNumber = num2,
									Title = current2.Attribute("title").Value,
									SectionTitle = sectionTitle,
									Folio = folio
								};
								if (dictionary.ContainsKey(num2))
								{
									List<TextArticle> list2 = dictionary[num2];
									list2.Add(item);
								}
								else
								{
									List<TextArticle> list3 = new List<TextArticle>();
									list3.Add(item);
									dictionary.Add(num2, list3);
								}
								articleSection.Articles.Add(item);
							}
						}
						list.Add(articleSection);
					}
					issue.PageNumberToArticlesMapping = dictionary;
					issue.TOCArticleSections = list;
				}
			}
			catch (Exception var_22_339)
			{
			}
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x000176A0 File Offset: 0x000158A0
		protected static string DecryptPdfPassword(string init, string encryptedPwd, string deviceId, string uuid)
		{
			return CryptoUtils.GetPdfPassword(init, encryptedPwd, deviceId, uuid);
		}
	}
}
