using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Globalization;
using Windows.Storage;
using ZinioReaderWin8.Model.Library;
using ZinioReaderWin8.Model.Reader;
using ZinioReaderWin8.Model.Shared;
using ZinioReaderWin8.Services;

namespace ZinioReaderWin8
{
	// Token: 0x020000BA RID: 186
	public class SettingManager
	{
		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000695 RID: 1685 RVA: 0x00029CBC File Offset: 0x00027EBC
		// (set) Token: 0x06000696 RID: 1686 RVA: 0x00029CC4 File Offset: 0x00027EC4
		public StorageFolder UsersFolder
		{
			get;
			private set;
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000697 RID: 1687 RVA: 0x00029CCD File Offset: 0x00027ECD
		// (set) Token: 0x06000698 RID: 1688 RVA: 0x00029CD5 File Offset: 0x00027ED5
		public StorageFolder MetadataFolder
		{
			get;
			private set;
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000699 RID: 1689 RVA: 0x00029CDE File Offset: 0x00027EDE
		// (set) Token: 0x0600069A RID: 1690 RVA: 0x00029CE6 File Offset: 0x00027EE6
		public StorageFolder ConfigFolder
		{
			get;
			private set;
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x0600069B RID: 1691 RVA: 0x00029CEF File Offset: 0x00027EEF
		// (set) Token: 0x0600069C RID: 1692 RVA: 0x00029CF7 File Offset: 0x00027EF7
		public StorageFolder LoginFolder
		{
			get;
			private set;
		}

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x0600069D RID: 1693 RVA: 0x00029D00 File Offset: 0x00027F00
		// (set) Token: 0x0600069E RID: 1694 RVA: 0x00029D08 File Offset: 0x00027F08
		public StorageFolder ReaderFolder
		{
			get;
			private set;
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x0600069F RID: 1695 RVA: 0x00029D11 File Offset: 0x00027F11
		// (set) Token: 0x060006A0 RID: 1696 RVA: 0x00029D19 File Offset: 0x00027F19
		public string LoginFilePath
		{
			get;
			private set;
		}

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x060006A1 RID: 1697 RVA: 0x00029D22 File Offset: 0x00027F22
		// (set) Token: 0x060006A2 RID: 1698 RVA: 0x00029D2A File Offset: 0x00027F2A
		public StorageFolder ExploreFolder
		{
			get;
			private set;
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x060006A3 RID: 1699 RVA: 0x00029D33 File Offset: 0x00027F33
		// (set) Token: 0x060006A4 RID: 1700 RVA: 0x00029D3B File Offset: 0x00027F3B
		public StorageFile ExploreFile
		{
			get;
			set;
		}

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x060006A5 RID: 1701 RVA: 0x00029D44 File Offset: 0x00027F44
		// (set) Token: 0x060006A6 RID: 1702 RVA: 0x00029D4C File Offset: 0x00027F4C
		public string ExploreFilePath
		{
			get;
			private set;
		}

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x060006A7 RID: 1703 RVA: 0x00029D55 File Offset: 0x00027F55
		// (set) Token: 0x060006A8 RID: 1704 RVA: 0x00029D5D File Offset: 0x00027F5D
		public StorageFolder ExcerptsFolder
		{
			get;
			private set;
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x060006A9 RID: 1705 RVA: 0x00029D66 File Offset: 0x00027F66
		// (set) Token: 0x060006AA RID: 1706 RVA: 0x00029D6E File Offset: 0x00027F6E
		public StorageFolder ShopFolder
		{
			get;
			private set;
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x060006AB RID: 1707 RVA: 0x00029D77 File Offset: 0x00027F77
		// (set) Token: 0x060006AC RID: 1708 RVA: 0x00029D7F File Offset: 0x00027F7F
		public StorageFile ShopFile
		{
			get;
			set;
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x060006AD RID: 1709 RVA: 0x00029D88 File Offset: 0x00027F88
		// (set) Token: 0x060006AE RID: 1710 RVA: 0x00029D90 File Offset: 0x00027F90
		public string ShopFilePath
		{
			get;
			private set;
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x060006AF RID: 1711 RVA: 0x00029D99 File Offset: 0x00027F99
		// (set) Token: 0x060006B0 RID: 1712 RVA: 0x00029DA1 File Offset: 0x00027FA1
		public StorageFolder TilesFolder
		{
			get;
			private set;
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x060006B1 RID: 1713 RVA: 0x00029DAA File Offset: 0x00027FAA
		// (set) Token: 0x060006B2 RID: 1714 RVA: 0x00029DB2 File Offset: 0x00027FB2
		public StorageFolder CurrentUserDirectory
		{
			get;
			private set;
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x060006B3 RID: 1715 RVA: 0x00029DBB File Offset: 0x00027FBB
		// (set) Token: 0x060006B4 RID: 1716 RVA: 0x00029DC3 File Offset: 0x00027FC3
		public StorageFile LibraryFile
		{
			get;
			set;
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x060006B5 RID: 1717 RVA: 0x00029DCC File Offset: 0x00027FCC
		// (set) Token: 0x060006B6 RID: 1718 RVA: 0x00029DD4 File Offset: 0x00027FD4
		public string LibraryFilePath
		{
			get;
			private set;
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x060006B7 RID: 1719 RVA: 0x00029DDD File Offset: 0x00027FDD
		// (set) Token: 0x060006B8 RID: 1720 RVA: 0x00029DE5 File Offset: 0x00027FE5
		public StorageFile SettingsFile
		{
			get;
			set;
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x060006B9 RID: 1721 RVA: 0x00029DEE File Offset: 0x00027FEE
		// (set) Token: 0x060006BA RID: 1722 RVA: 0x00029DF6 File Offset: 0x00027FF6
		public StorageFile AnonSettingsFile
		{
			get;
			set;
		}

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x060006BB RID: 1723 RVA: 0x00029DFF File Offset: 0x00027FFF
		// (set) Token: 0x060006BC RID: 1724 RVA: 0x00029E07 File Offset: 0x00028007
		public StorageFile ReadingsFile
		{
			get;
			set;
		}

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x060006BD RID: 1725 RVA: 0x00029E10 File Offset: 0x00028010
		// (set) Token: 0x060006BE RID: 1726 RVA: 0x00029E18 File Offset: 0x00028018
		public StorageFile ConfigFile
		{
			get;
			set;
		}

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x060006BF RID: 1727 RVA: 0x00029E21 File Offset: 0x00028021
		// (set) Token: 0x060006C0 RID: 1728 RVA: 0x00029E29 File Offset: 0x00028029
		public StorageFolder BookmarksFolder
		{
			get;
			set;
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x060006C1 RID: 1729 RVA: 0x00029E32 File Offset: 0x00028032
		// (set) Token: 0x060006C2 RID: 1730 RVA: 0x00029E3A File Offset: 0x0002803A
		public StorageFile BookmarksFile
		{
			get;
			set;
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x060006C3 RID: 1731 RVA: 0x00029E43 File Offset: 0x00028043
		// (set) Token: 0x060006C4 RID: 1732 RVA: 0x00029E4B File Offset: 0x0002804B
		public StorageFile ArchiveFile
		{
			get;
			set;
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x060006C5 RID: 1733 RVA: 0x00029E54 File Offset: 0x00028054
		// (set) Token: 0x060006C6 RID: 1734 RVA: 0x00029E5C File Offset: 0x0002805C
		public string OEMApplicationName
		{
			get;
			set;
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x060006C7 RID: 1735 RVA: 0x00029E65 File Offset: 0x00028065
		// (set) Token: 0x060006C8 RID: 1736 RVA: 0x00029E6D File Offset: 0x0002806D
		public string InstallationUUID
		{
			get;
			private set;
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x060006C9 RID: 1737 RVA: 0x00029E76 File Offset: 0x00028076
		// (set) Token: 0x060006CA RID: 1738 RVA: 0x00029E7E File Offset: 0x0002807E
		public bool IsUserSignIn
		{
			get;
			private set;
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x060006CB RID: 1739 RVA: 0x00029E87 File Offset: 0x00028087
		// (set) Token: 0x060006CC RID: 1740 RVA: 0x00029E8F File Offset: 0x0002808F
		public Profile Profile
		{
			get
			{
				return this._profile;
			}
			private set
			{
				this._profile = value;
				this.SaveProfile();
			}
		}

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x060006CD RID: 1741 RVA: 0x00029E9E File Offset: 0x0002809E
		// (set) Token: 0x060006CE RID: 1742 RVA: 0x00029EA6 File Offset: 0x000280A6
		public string SystemRegion_TwoLetterCode
		{
			get;
			private set;
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x060006CF RID: 1743 RVA: 0x00029EAF File Offset: 0x000280AF
		// (set) Token: 0x060006D0 RID: 1744 RVA: 0x00029EB7 File Offset: 0x000280B7
		public string SystemRegion_NativeName
		{
			get;
			private set;
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x060006D1 RID: 1745 RVA: 0x00029EC0 File Offset: 0x000280C0
		// (set) Token: 0x060006D2 RID: 1746 RVA: 0x00029EC8 File Offset: 0x000280C8
		public string SystemLanguage_Language
		{
			get;
			private set;
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x060006D3 RID: 1747 RVA: 0x00029ED1 File Offset: 0x000280D1
		// (set) Token: 0x060006D4 RID: 1748 RVA: 0x00029ED9 File Offset: 0x000280D9
		public string SystemLanguage_Region
		{
			get;
			private set;
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x060006D5 RID: 1749 RVA: 0x00029EE2 File Offset: 0x000280E2
		// (set) Token: 0x060006D6 RID: 1750 RVA: 0x00029EEA File Offset: 0x000280EA
		public string DeviceId
		{
			get;
			private set;
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x060006D7 RID: 1751 RVA: 0x00029EF3 File Offset: 0x000280F3
		// (set) Token: 0x060006D8 RID: 1752 RVA: 0x00029EFB File Offset: 0x000280FB
		public string Locale
		{
			get;
			private set;
		}

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x060006D9 RID: 1753 RVA: 0x00029F04 File Offset: 0x00028104
		// (set) Token: 0x060006DA RID: 1754 RVA: 0x00029F0C File Offset: 0x0002810C
		public string DeviceName
		{
			get;
			private set;
		}

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x060006DB RID: 1755 RVA: 0x00029F15 File Offset: 0x00028115
		// (set) Token: 0x060006DC RID: 1756 RVA: 0x00029F1D File Offset: 0x0002811D
		public string OsVersion
		{
			get;
			private set;
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x060006DD RID: 1757 RVA: 0x00029F26 File Offset: 0x00028126
		// (set) Token: 0x060006DE RID: 1758 RVA: 0x00029F2E File Offset: 0x0002812E
		public string DefaultNewsstandId
		{
			get;
			private set;
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x060006DF RID: 1759 RVA: 0x00029F37 File Offset: 0x00028137
		// (set) Token: 0x060006E0 RID: 1760 RVA: 0x00029F3F File Offset: 0x0002813F
		public bool NewsstandOverride
		{
			get;
			private set;
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x060006E1 RID: 1761 RVA: 0x00029F48 File Offset: 0x00028148
		// (set) Token: 0x060006E2 RID: 1762 RVA: 0x00029F50 File Offset: 0x00028150
		public string DefaultAppId
		{
			get;
			private set;
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x060006E3 RID: 1763 RVA: 0x00029F59 File Offset: 0x00028159
		// (set) Token: 0x060006E4 RID: 1764 RVA: 0x00029F61 File Offset: 0x00028161
		public ObservableCollection<Newsstand> Newsstands
		{
			get;
			private set;
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x060006E5 RID: 1765 RVA: 0x00029F6A File Offset: 0x0002816A
		// (set) Token: 0x060006E6 RID: 1766 RVA: 0x00029F72 File Offset: 0x00028172
		public string DefaultCommerceURL
		{
			get;
			private set;
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x060006E7 RID: 1767 RVA: 0x00029F7B File Offset: 0x0002817B
		// (set) Token: 0x060006E8 RID: 1768 RVA: 0x00029F83 File Offset: 0x00028183
		public ReadingInfo ReadingSettings
		{
			get
			{
				return this.LoadReadingInfo();
			}
			private set
			{
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x060006E9 RID: 1769 RVA: 0x00029F85 File Offset: 0x00028185
		// (set) Token: 0x060006EA RID: 1770 RVA: 0x00029F8D File Offset: 0x0002818D
		public ObservableCollection<Bookmark> Bookmarks
		{
			get;
			private set;
		}

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x060006EB RID: 1771 RVA: 0x00029F96 File Offset: 0x00028196
		// (set) Token: 0x060006EC RID: 1772 RVA: 0x00029F9E File Offset: 0x0002819E
		public SettingsInfo UserSettings
		{
			get;
			set;
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x060006ED RID: 1773 RVA: 0x00029FA7 File Offset: 0x000281A7
		// (set) Token: 0x060006EE RID: 1774 RVA: 0x00029FAF File Offset: 0x000281AF
		public SettingsInfo AnonSettings
		{
			get;
			set;
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x060006EF RID: 1775 RVA: 0x00029FB8 File Offset: 0x000281B8
		// (set) Token: 0x060006F0 RID: 1776 RVA: 0x00029FC0 File Offset: 0x000281C0
		public bool IsFirstTime
		{
			get;
			set;
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x00029FC9 File Offset: 0x000281C9
		public int GetFontSize()
		{
			if (this.UserSettings != null)
			{
				return this.UserSettings.FontSize;
			}
			return this.AnonSettings.FontSize;
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x00029FEA File Offset: 0x000281EA
		public void SetFontSize(int size)
		{
			if (this.UserSettings == null)
			{
				this.AnonSettings.FontSize = size;
				return;
			}
			this.UserSettings.FontSize = size;
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x060006F3 RID: 1779 RVA: 0x0002A00D File Offset: 0x0002820D
		public static SettingManager Instance
		{
			get
			{
				if (SettingManager._instance == null)
				{
					SettingManager._instance = new SettingManager();
				}
				return SettingManager._instance;
			}
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x0002A108 File Offset: 0x00028308
		public static async Task InitSettingManager()
		{
			if (SettingManager._instance == null)
			{
				SettingManager._instance = new SettingManager();
			}
			await SettingManager._instance.Init();
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x0002A146 File Offset: 0x00028346
		private SettingManager()
		{
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x0002A360 File Offset: 0x00028560
		private async Task Init()
		{
			await this.InitDefaultSettings();
			this.FetchSystemInfo();
			this._profile = await this.LoadUserProfile();
			if (this._profile != null)
			{
				this.IsUserSignIn = true;
			}
			await this.UpdateUserSettings();
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x0002A410 File Offset: 0x00028610
		public async Task Reset()
		{
			this.InitDefaultSettings();
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x0002AE5C File Offset: 0x0002905C
		private async Task InitDefaultSettings()
		{
			ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
			if (localSettings.Values["IsFirstTime"] == null)
			{
				this.IsFirstTime = true;
				localSettings.Values["IsFirstTime"] = true;
			}
			else
			{
				this.IsFirstTime = false;
			}
			this.InstallationUUID = "4f867a26-8cf9-4a5b-a15a-9eba77365031";
			this.DeviceId = "4860DBAD-768D-40B5-8BD0-F60224B451D5";
			this.DefaultNewsstandId = ((localSettings.Values["CurrentNewsstandId"] != null) ? localSettings.Values["CurrentNewsstandId"].ToString() : null);
			this.NewsstandOverride = (localSettings.Values["NewsstandOverride"] != null);
			this.DefaultAppId = "501";
			ApplicationData current = ApplicationData.Current;
			StorageFolder localFolder = current.LocalFolder;
			this.MetadataFolder = await localFolder.CreateFolderAsync("metadata", CreationCollisionOption.OpenIfExists);
			try
			{
				StorageFile storageFile = await Package.Current.InstalledLocation.GetFileAsync("microsoft.system.package.metadata\\Custom.data");
				if (storageFile != null)
				{
					XElement xElement = XElement.Load(await storageFile.OpenStreamForReadAsync());
					this.OEMApplicationName = xElement.Descendants("ZinioAppName").FirstOrDefault<XElement>().Value;
				}
				else
				{
					this.OEMApplicationName = "";
				}
			}
			catch
			{
				this.OEMApplicationName = "";
			}
			this.ConfigFolder = await this.MetadataFolder.CreateFolderAsync("config", CreationCollisionOption.OpenIfExists);
			this.UsersFolder = await this.MetadataFolder.CreateFolderAsync("users", CreationCollisionOption.OpenIfExists);
			this.ReaderFolder = await this.MetadataFolder.CreateFolderAsync("Reader", CreationCollisionOption.OpenIfExists);
			this.LoginFolder = await this.ReaderFolder.CreateFolderAsync("login", CreationCollisionOption.OpenIfExists);
			this.LoginFilePath = this.LoginFolder.Path + "\\credentials";
			this.ExploreFolder = await localFolder.CreateFolderAsync("Explore", CreationCollisionOption.OpenIfExists);
			this.ExploreFilePath = this.ExploreFolder.Path + "\\explore.xml";
			this.ShopFolder = await localFolder.CreateFolderAsync("Shop", CreationCollisionOption.OpenIfExists);
			this.ShopFilePath = this.ShopFolder.Path + "\\shop.xml";
			this.ExcerptsFolder = await localFolder.CreateFolderAsync("Excerpts", CreationCollisionOption.OpenIfExists);
			this.TilesFolder = await localFolder.CreateFolderAsync("Tiles", CreationCollisionOption.OpenIfExists);
			if (localSettings.Values["CacheClearedOn"] != null)
			{
				TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks - (long)localSettings.Values["CacheClearedOn"]);
				if (timeSpan.Minutes > 1440)
				{
					this.ClearAllCache(10080, true, false);
					localSettings.Values["CacheClearedOn"] = DateTime.Now.Ticks;
				}
			}
			else
			{
				localSettings.Values["CacheClearedOn"] = DateTime.Now.Ticks;
			}
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x0002AEA2 File Offset: 0x000290A2
		private void FetchSystemInfo()
		{
			this.DeviceName = "Windows 8 Device";
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x0002B1F8 File Offset: 0x000293F8
		private async Task<Profile> LoadUserProfile()
		{
			Profile result;
			if (!(await Utils.DoesFileExistAsync(this.LoginFilePath, false)))
			{
				result = null;
			}
			else
			{
				try
				{
					StorageFile storageFile = await StorageFile.GetFileFromPathAsync(this.LoginFilePath);
					byte[] encryptedContent = await Utils.ReadAllBytes(storageFile);
					string text = Utils.DecryptString(encryptedContent);
					if (text == null)
					{
						result = null;
						return result;
					}
					XElement xElement = XElement.Parse(text);
					if (xElement != null)
					{
						try
						{
							Profile profile = new Profile
							{
								Email = xElement.Descendants("email").First<XElement>().Value,
								ProfileId = xElement.Descendants("profileId").First<XElement>().Value,
								UserDeviceId = xElement.Descendants("userDeviceId").First<XElement>().Value,
								SigninDeviceId = xElement.Descendants("signinDeviceId").First<XElement>().Value,
								ReaderVersion = xElement.Descendants("readerVersion").First<XElement>().Value
							};
							result = profile;
							return result;
						}
						catch (Exception)
						{
						}
					}
				}
				catch (Exception)
				{
					throw;
				}
				result = null;
			}
			return result;
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x0002B9EC File Offset: 0x00029BEC
		private async Task UpdateUserSettings()
		{
			if (this.Profile == null)
			{
				this.LibraryFile = null;
				this.SettingsFile = null;
				this.UserSettings = null;
				this.ReadingsFile = null;
				this.ReadingSettings = null;
				this.BookmarksFolder = null;
				this.BookmarksFile = null;
				this.Bookmarks = null;
				this.ConfigFile = await this.ConfigFolder.CreateFileAsync("config.xml", CreationCollisionOption.OpenIfExists);
				this.UpdateConfigurationSettings();
				this.AnonSettingsFile = await this.UsersFolder.CreateFileAsync("AnonymousUser", CreationCollisionOption.OpenIfExists);
				this.AnonSettings = await SettingManager.LoadSettingsInfo(true);
			}
			else
			{
				this.CurrentUserDirectory = await this.UsersFolder.CreateFolderAsync(this.Profile.ProfileId, CreationCollisionOption.OpenIfExists);
				this.BookmarksFolder = await this.CurrentUserDirectory.CreateFolderAsync("Bookmarks", CreationCollisionOption.OpenIfExists);
				this.LoadBookmarks();
				this.LibraryFile = await this.CurrentUserDirectory.CreateFileAsync("library.xml", CreationCollisionOption.OpenIfExists);
				this.LibraryFilePath = this.LibraryFile.Path;
				this.SettingsFile = await this.CurrentUserDirectory.CreateFileAsync("settings.xml", CreationCollisionOption.OpenIfExists);
				this.ReadingsFile = await this.CurrentUserDirectory.CreateFileAsync("readings.xml", CreationCollisionOption.OpenIfExists);
				this.ReadingSettings = this.LoadReadingInfo();
				this.UserSettings = await SettingManager.LoadSettingsInfo(false);
				this.ConfigFile = await this.CurrentUserDirectory.CreateFileAsync("config.xml", CreationCollisionOption.OpenIfExists);
				this.UpdateConfigurationSettings();
			}
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x0002BEAC File Offset: 0x0002A0AC
		private async void LoadBookmarks()
		{
			bool flag = false;
			try
			{
				if (this.BookmarksFile == null)
				{
					this.BookmarksFile = await this.CurrentUserDirectory.CreateFileAsync("bookmarks.xml", CreationCollisionOption.OpenIfExists);
				}
				XElement xElement = XElement.Load(await this.BookmarksFile.OpenStreamForReadAsync());
				this.Bookmarks = new ObservableCollection<Bookmark>(from ec in xElement.Descendants("bookmark")
				select new Bookmark
				{
					PublicationId = ec.Element("publicationId").Value,
					PublicationName = ec.Element("publicationName").Value,
					PublicationDate = ec.Element("publicationDate").Value,
					IssueId = ec.Element("issueId").Value,
					CategoryId = ec.Element("categoryId").Value,
					ExcerptId = ec.Element("excerptId").Value,
					ArticleName = ec.Element("articleName").Value,
					PageNumber = ec.Element("pageNumber").Value,
					ThumbnailUrl = ec.Element("thumbnailUrl").Value,
					Created = new DateTime(long.Parse(ec.Element("created").Value))
				});
			}
			catch (XmlException)
			{
				flag = true;
			}
			catch (Exception)
			{
				this.Bookmarks = new ObservableCollection<Bookmark>();
			}
			if (flag)
			{
				try
				{
					XElement xElement2 = new XElement("bookmarks");
					string contents = xElement2.ToString(SaveOptions.None);
					await FileIO.WriteTextAsync(this.BookmarksFile, contents);
					this.Bookmarks = new ObservableCollection<Bookmark>();
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x0002C084 File Offset: 0x0002A284
		public async void SaveBookmarks()
		{
			try
			{
				XElement xElement = new XElement("bookmarks");
				foreach (Bookmark current in this.Bookmarks)
				{
					xElement.Add(current.ToXmlElement());
				}
				string contents = xElement.ToString(SaveOptions.None);
				await FileIO.WriteTextAsync(this.BookmarksFile, contents);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x0002C0C0 File Offset: 0x0002A2C0
		private ReadingInfo LoadReadingInfo()
		{
			try
			{
				if (this.ReadingsFile != null)
				{
					XElement xElement = XElement.Load(this.ReadingsFile.Path);
					return new ReadingInfo
					{
						LastReadPubId = xElement.Descendants("lastReadPubId").FirstOrDefault<XElement>().Value,
						LastReadIssueId = xElement.Descendants("lastReadIssueId").FirstOrDefault<XElement>().Value,
						LastReadFolio = xElement.Descendants("lastReadFolio").FirstOrDefault<XElement>().Value
					};
				}
			}
			catch (Exception)
			{
			}
			return null;
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x0002C4DC File Offset: 0x0002A6DC
		private async void SaveProfile()
		{
			if (this._profile != null)
			{
				XElement xElement = new XElement("profile", new object[]
				{
					new XElement("email", this._profile.Email),
					new XElement("profileId", this._profile.ProfileId),
					new XElement("userDeviceId", this._profile.UserDeviceId),
					new XElement("signinDeviceId", this._profile.SigninDeviceId),
					new XElement("readerVersion", this._profile.ReaderVersion)
				});
				StorageFile file = null;
				if (await Utils.DoesFileExistAsync(this.LoginFilePath, false))
				{
					file = await StorageFile.GetFileFromPathAsync(this.LoginFilePath);
				}
				else
				{
					file = await this.LoginFolder.CreateFileAsync("credentials");
				}
				string plainText = xElement.ToString(SaveOptions.None);
				byte[] buffer = Utils.EncryptString(plainText);
				await FileIO.WriteBytesAsync(file, buffer);
			}
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x0002C518 File Offset: 0x0002A718
		public static string ProcessCommerceUrl(string url, string email = "", string userDeviceId = "", string productId = "", string skuID = "", string categoryId = "")
		{
			StringBuilder stringBuilder = new StringBuilder(url);
			stringBuilder.Replace("[email]", email);
			stringBuilder.Replace("[userDeviceId]", userDeviceId);
			stringBuilder.Replace("[productId]", productId);
			stringBuilder.Replace("[skuID]", skuID);
			stringBuilder.Replace("[categoryId]]", categoryId);
			stringBuilder.Replace("[categoryId]", categoryId);
			return stringBuilder.ToString();
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x0002C8AC File Offset: 0x0002AAAC
		public async Task ClearExploreCache(int expiryInMinutes = 0)
		{
			try
			{
				IReadOnlyList<StorageFolder> readOnlyList = await this.ExploreFolder.GetFoldersAsync();
				foreach (StorageFolder current in readOnlyList)
				{
					if (expiryInMinutes > 0)
					{
						if (await Utils.IsFileExpired(current.Path, expiryInMinutes, true))
						{
							await current.DeleteAsync(StorageDeleteOption.PermanentDelete);
						}
					}
					else
					{
						await current.DeleteAsync(StorageDeleteOption.PermanentDelete);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x0002CC24 File Offset: 0x0002AE24
		public async Task ClearExcerptCache(int expiryInMinutes = 0)
		{
			try
			{
				IReadOnlyList<StorageFolder> readOnlyList = await this.ExcerptsFolder.GetFoldersAsync();
				foreach (StorageFolder current in readOnlyList)
				{
					if (expiryInMinutes > 0)
					{
						if (await Utils.IsFileExpired(current.Path, expiryInMinutes, true))
						{
							await current.DeleteAsync(StorageDeleteOption.PermanentDelete);
						}
					}
					else
					{
						await current.DeleteAsync(StorageDeleteOption.PermanentDelete);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x0002CF9C File Offset: 0x0002B19C
		public async Task ClearShopCache(int expiryInMinutes = 0)
		{
			try
			{
				IReadOnlyList<StorageFolder> readOnlyList = await this.ShopFolder.GetFoldersAsync();
				foreach (StorageFolder current in readOnlyList)
				{
					if (expiryInMinutes > 0)
					{
						if (await Utils.IsFileExpired(current.Path, expiryInMinutes, true))
						{
							await current.DeleteAsync(StorageDeleteOption.PermanentDelete);
						}
					}
					else
					{
						await current.DeleteAsync(StorageDeleteOption.PermanentDelete);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x0002D420 File Offset: 0x0002B620
		public async Task ClearAllCache(int expiryInMinutes = 0, bool async = true, bool completeClean = false)
		{
			if (completeClean)
			{
				try
				{
					await SettingManager.Instance.ExcerptsFolder.DeleteAsync();
					await SettingManager.Instance.ExploreFolder.DeleteAsync();
					await SettingManager.Instance.ShopFolder.DeleteAsync();
					await SettingManager.Instance.Reset();
					return;
				}
				catch (Exception)
				{
					return;
				}
			}
			if (async)
			{
				this.ClearExploreCache(expiryInMinutes);
				this.ClearShopCache(expiryInMinutes);
				this.ClearExcerptCache(expiryInMinutes);
			}
			else
			{
				await this.ClearExploreCache(expiryInMinutes);
				await this.ClearShopCache(expiryInMinutes);
				await this.ClearExcerptCache(expiryInMinutes);
			}
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x0002D480 File Offset: 0x0002B680
		private void DetermineSystemLocale()
		{
			GeographicRegion geographicRegion = new GeographicRegion();
			string codeTwoLetter = geographicRegion.CodeTwoLetter;
			string text = ApplicationLanguages.Languages[0];
			string text2 = text.Split(new char[]
			{
				'-'
			})[0];
			this.Locale = text2 + "_" + codeTwoLetter;
			ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
			bool flag = false;
			this.NewsstandOverride = (localSettings.Values["NewsstandOverride"] != null);
			object obj = localSettings.Values["regionCode"];
			if (obj != null && !flag)
			{
				flag = !obj.Equals(codeTwoLetter);
			}
			localSettings.Values["regionCode"] = (this.SystemRegion_TwoLetterCode = codeTwoLetter);
			this.SystemRegion_NativeName = geographicRegion.NativeName;
			object obj2 = localSettings.Values["pLanguage_Language"];
			if (obj2 != null && !flag)
			{
				flag = !obj2.Equals(text2);
			}
			localSettings.Values["pLanguage_Language"] = (this.SystemLanguage_Language = text2);
			object arg_117_0 = localSettings.Values["pLanguage_Region"];
			if (!this.NewsstandOverride && flag)
			{
				this.ResetNewsstand(null);
			}
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x0002D778 File Offset: 0x0002B978
		public async Task ResetNewsstand(string newsstandId)
		{
			ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
			if (newsstandId != null)
			{
				localSettings.Values["NewsstandOverride"] = true;
			}
			localSettings.Values["CurrentNewsstandId"] = newsstandId;
			this.DefaultNewsstandId = newsstandId;
			AppManager.Instance.Shop = null;
			await AppManager.Instance.ResetExplore();
			await this.UpdateUserSettings();
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x0002DC24 File Offset: 0x0002BE24
		private async void UpdateConfigurationSettings()
		{
			try
			{
				this.DetermineSystemLocale();
				Configuration configuration = await WebServiceHelper.Instance.GetConfiguration();
				if (configuration != null)
				{
					this.Newsstands = configuration.Newsstands;
					if (this.Profile == null)
					{
						string arg_EC_0 = string.Empty;
					}
					else
					{
						string arg_FF_0 = this.Profile.Email;
					}
					if (this.Profile == null)
					{
						string arg_112_0 = string.Empty;
					}
					else
					{
						string arg_125_0 = this.Profile.UserDeviceId;
					}
					if (this.Newsstands != null)
					{
						foreach (Newsstand current in this.Newsstands)
						{
							if ((!this.NewsstandOverride && current.IsDefault) || (this.NewsstandOverride && current.Id.Equals(SettingManager.Instance.DefaultNewsstandId)))
							{
								this.DefaultNewsstandId = current.Id;
								ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
								localSettings.Values["CurrentNewsstandId"] = this.DefaultNewsstandId;
								this.DefaultCommerceURL = current.CommerceURL;
								break;
							}
						}
					}
					XElement xElement = new XElement("configuration", new object[]
					{
						new XElement("DefaultNewsstandId", this.DefaultNewsstandId),
						new XElement("DefaultCommerceURL", this.DefaultCommerceURL)
					});
					string contents = xElement.ToString(SaveOptions.None);
					await FileIO.WriteTextAsync(this.ConfigFile, contents);
				}
				else if (SettingManager._instance.ConfigFile != null)
				{
					XElement xElement2 = XElement.Load(await SettingManager._instance.ConfigFile.OpenStreamForReadAsync());
					this.DefaultNewsstandId = xElement2.Descendants("DefaultNewsstandId").FirstOrDefault<XElement>().Value;
					this.DefaultCommerceURL = xElement2.Descendants("DefaultCommerceURL").FirstOrDefault<XElement>().Value;
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x0002DC60 File Offset: 0x0002BE60
		public string CurrentVersion()
		{
			Package current = Package.Current;
			return current.Id.Version.ToString();
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x0002DD78 File Offset: 0x0002BF78
		public async Task UpdateLoggedInUser(Profile profile)
		{
			this.Profile = profile;
			await this.UpdateUserSettings();
			this.IsUserSignIn = true;
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x0002DEA8 File Offset: 0x0002C0A8
		public async Task SignOutAndResetDeviceID()
		{
			await this.SignOut();
			Utils.ResetDeviceId(this.DeviceId);
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x0002E154 File Offset: 0x0002C354
		public async Task SignOut()
		{
			this.Profile = null;
			if (await Utils.DoesFileExistAsync(this.LoginFilePath, false))
			{
				StorageFile storageFile = await StorageFile.GetFileFromPathAsync(this.LoginFilePath);
				await storageFile.DeleteAsync();
			}
			await this.UpdateUserSettings();
			this.IsUserSignIn = false;
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x0002E19A File Offset: 0x0002C39A
		public bool IsDeviceIDInvalid()
		{
			return !this.DeviceId.Equals(this.Profile.SigninDeviceId);
		}

		// Token: 0x0600070D RID: 1805 RVA: 0x0002E310 File Offset: 0x0002C510
		public async void SaveReadingInfo(string pubId, string issueId, string folio)
		{
			XElement xElement = new XElement("readings", new object[]
			{
				new XElement("lastReadPubId", pubId),
				new XElement("lastReadIssueId", issueId),
				new XElement("lastReadFolio", folio)
			});
			string contents = xElement.ToString(SaveOptions.None);
			await FileIO.WriteTextAsync(this.ReadingsFile, contents);
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x0002E450 File Offset: 0x0002C650
		public async void CleanupReadingInfo()
		{
			if (this.ReadingsFile != null)
			{
				await this.ReadingsFile.DeleteAsync();
			}
			this.ReadingSettings = null;
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x0002E7EC File Offset: 0x0002C9EC
		public static async Task<SettingsInfo> LoadSettingsInfo(bool isAnon = false)
		{
			SettingsInfo result;
			try
			{
				if (SettingManager._instance.SettingsFile != null)
				{
					XElement xElement = XElement.Load(await SettingManager._instance.SettingsFile.OpenStreamForReadAsync());
					SettingsInfo settingsInfo = new SettingsInfo
					{
						AutoDownloadDays = Convert.ToInt32(xElement.Descendants("AutoDownloadDays").FirstOrDefault<XElement>().Value),
						AutoDeleteDays = Convert.ToInt32(xElement.Descendants("AutoDeleteDays").FirstOrDefault<XElement>().Value),
						FontSize = Convert.ToInt32(xElement.Descendants("FontSize").FirstOrDefault<XElement>().Value)
					};
					result = settingsInfo;
					return result;
				}
				if (isAnon)
				{
					XElement xElement2 = XElement.Load(await SettingManager._instance.AnonSettingsFile.OpenStreamForReadAsync());
					SettingsInfo settingsInfo2 = new SettingsInfo
					{
						AutoDownloadDays = Convert.ToInt32(xElement2.Descendants("AutoDownloadDays").FirstOrDefault<XElement>().Value),
						AutoDeleteDays = Convert.ToInt32(xElement2.Descendants("AutoDeleteDays").FirstOrDefault<XElement>().Value),
						FontSize = Convert.ToInt32(xElement2.Descendants("FontSize").FirstOrDefault<XElement>().Value)
					};
					result = settingsInfo2;
					return result;
				}
			}
			catch (Exception)
			{
			}
			result = new SettingsInfo
			{
				AutoDownloadDays = 0,
				AutoDeleteDays = 0,
				FontSize = 0
			};
			return result;
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x0002EB98 File Offset: 0x0002CD98
		public async void SaveSettings()
		{
			try
			{
				if (this.SettingsFile != null)
				{
					XElement xElement = new XElement("settings", new object[]
					{
						new XElement("AutoDownloadDays", this.UserSettings.AutoDownloadDays),
						new XElement("AutoDeleteDays", this.UserSettings.AutoDeleteDays),
						new XElement("FontSize", this.UserSettings.FontSize)
					});
					string contents = xElement.ToString(SaveOptions.None);
					await FileIO.WriteTextAsync(this.SettingsFile, contents);
				}
				else
				{
					XElement xElement2 = new XElement("settings", new object[]
					{
						new XElement("AutoDownloadDays", 0),
						new XElement("AutoDeleteDays", 0),
						new XElement("FontSize", this.AnonSettings.FontSize)
					});
					string contents2 = xElement2.ToString(SaveOptions.None);
					await FileIO.WriteTextAsync(this.AnonSettingsFile, contents2);
				}
			}
			catch (Exception)
			{
			}
			await SettingManager.LoadSettingsInfo(this.SettingsFile != null);
		}

		// Token: 0x06000711 RID: 1809 RVA: 0x0002ECC0 File Offset: 0x0002CEC0
		public async void CleanupSettingsInfo()
		{
			if (this.SettingsFile != null)
			{
				await this.SettingsFile.DeleteAsync();
			}
			this.SettingsFile = null;
		}

		// Token: 0x04000370 RID: 880
		private const string LIBRARY_FILENAME = "library.xml";

		// Token: 0x04000371 RID: 881
		private const string SETTINGS_FILENAME = "settings.xml";

		// Token: 0x04000372 RID: 882
		private const string CREDENTIALS_FILENAME = "credentials";

		// Token: 0x04000373 RID: 883
		private const string EXPLORE_FILENAME = "explore.xml";

		// Token: 0x04000374 RID: 884
		private const string READINGS_FILENAME = "readings.xml";

		// Token: 0x04000375 RID: 885
		private const string CONFIG_FILENAME = "config.xml";

		// Token: 0x04000376 RID: 886
		private const string BOOKMARKS_FILENAME = "bookmarks.xml";

		// Token: 0x04000377 RID: 887
		private const string ARCHIVE_FILENAME = "archive.xml";

		// Token: 0x04000378 RID: 888
		private const string DEFAULT_NEWSSTAND_ID = null;

		// Token: 0x04000379 RID: 889
		private const string DEFAULT_APP_ID = "501";

		// Token: 0x0400037A RID: 890
		private const string ANONYMOUS_USER_FOLDERNAME = "AnonymousUser";

		// Token: 0x0400037B RID: 891
		private static SettingManager _instance;

		// Token: 0x0400037C RID: 892
		private Profile _profile;

		// Token: 0x0400037D RID: 893
		private ReadingInfo _readingSettings;
	}
}
