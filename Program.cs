using Microsoft.Playwright;
using PlaywrightDemo;

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new() { Channel = "msedge", Headless = false });
var page = await browser.NewPageAsync();
await page.AddInitScriptAsync(scriptPath: @"N:\.NET CORE\Xiudong-Playwright\Xiudong-Playwright\stealth.min.js");
await page.GotoAsync("https://wap.showstart.com/pages/passport/login/login?redirect=%2Fpages%2FmyHome%2FmyHome");

var helper = new Helper("0fc7521c6836ac036a39b1dabace907e", "182990", false, null, "1", 1);
await helper.LoadTicketPage(page);