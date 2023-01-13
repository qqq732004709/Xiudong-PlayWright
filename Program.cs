using Microsoft.Playwright;
using PlaywrightDemo;

//加载配置
var config = AppConfig.Load();

//定时运行
if (config.StartTime.HasValue && config.StartTime > DateTime.Now)
{
    var timeSpan = config.StartTime - DateTime.Now;
    //只允许定时十分钟内的任务
    if (timeSpan.Value.TotalMinutes > 10)
    {
        Console.WriteLine("只允许定时十分钟内的任务");
        return;
    }

    using var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
    while (await timer.WaitForNextTickAsync())
    {
        if (DateTime.Now.AddSeconds(-10) >= config.StartTime.Value)
        {
            Console.WriteLine($"当前时间：{DateTime.Now} 开始执行任务");
            timer.Dispose();
            break;
        }
        Console.WriteLine($"当前时间：{DateTime.Now} 未到预约开始时间");
    }
}
Console.WriteLine($"当前时间：{DateTime.Now} 任务开始");

var authPath = "auth.json";
AppConfig.CheckAuthFile(authPath);

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new() { Channel = "msedge", Headless = false });
var context = await browser.NewContextAsync(new BrowserNewContextOptions() { StorageStatePath = authPath });

var page = await context.NewPageAsync();
await page.AddInitScriptAsync(scriptPath: @"stealth.min.js");

var helper = new Helper(page, config);
var isLogin = await helper.CheckLogin();

if (!isLogin)
{
    Console.WriteLine("登录失败");
    return;
}

await context.StorageStateAsync(new BrowserContextStorageStateOptions() { Path = authPath });
await helper.PurchaseLoop();

await context.CloseAsync();
await browser.CloseAsync();