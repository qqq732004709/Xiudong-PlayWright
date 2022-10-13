using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace PlaywrightDemo;
public class AppConfig
{
    /// <summary>
    /// 配置文件名（必须跟程序同目录）
    /// </summary>
    public const string CONFIG_FILE_NAME = "ticketconfig.json";

    public string ActivityId; //live场次id
    public string TicketId;  //购票分类id
    public string TicketNum; //购买数量
    public DateTime? StartTime; //开始抢购时间
    public bool NeedSelect; //是否选择购票人
    public int SelectNum; //选择数量

    public AppConfig()
    {
        NeedSelect = false;
        SelectNum = 1;
        StartTime = null;
        TicketNum = "1";
        TicketId = "";
        ActivityId = "";
    }


    /// <summary>
    /// 从本地加载配置
    /// </summary>
    /// <returns>返回从本地加载的程序配置，本地没有则返回默认配置（并自动保存它到本地）</returns>
    public static AppConfig Load()
    {
        AppConfig? config = null;

        try
        {
            //从文件中读取
            string configContent = File.ReadAllText(CONFIG_FILE_NAME);
            config = JsonConvert.DeserializeObject<AppConfig>(configContent);
        }
        catch (FileNotFoundException)
        {
            //本地没有配置文件，直接忽略该异常
        }

        if (config == null)
        {
            config = GetDefaultConfig();
            Save(config);
            Console.WriteLine("初始化配置成功");
        }

        return config;
    }
  

    /// <summary>
    /// 保存配置到本地
    /// </summary>
    /// <param name="config">要保存的程序配置，传入null等效于清空原文件</param>
    public static void Save(AppConfig config)
    {
        string configContent = config == null ? string.Empty
            : JsonConvert.SerializeObject(config);
        //保存到文件中
        File.WriteAllText(CONFIG_FILE_NAME, configContent);
    }

    /// <summary>
    /// 首次获取初始配置时不要直接使用此方法，使用load()方法加载配置！
    /// </summary>
    /// <param name="loadFailed">表示是否因为加载本地配置失败才调用该方法</param>
    /// <returns>返回默认配置对象</returns>
    private static AppConfig GetDefaultConfig(bool loadFailed = false)
    {
        return new AppConfig()
        {
            ActivityId = "0fc7521c6836ac036a39b1dabace907e",
            TicketId= "182990"
        };
    }

    public static void CheckAuthFile(string authPath)
    {
        bool ifExistsAuth = File.Exists(authPath);
        if (!ifExistsAuth)
        {
            using var file = File.Create(authPath);
        }
    }
}
