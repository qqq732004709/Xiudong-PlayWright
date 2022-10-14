using System;
using Microsoft.Playwright;

namespace PlaywrightDemo;

public class Helper
{
    public string _activityId;
    public string _ticketId;
    public string _ticketNum;
    public DateTime? _startTime;
    public bool _needSelect;
    public int _selectNum;
    public IPage page;

    public Helper(IPage page,
                 AppConfig appConfig)
    {
        this.page = page;
        _activityId = appConfig.ActivityId;
        _ticketId = appConfig.TicketId;
        _ticketNum = appConfig.TicketNum;
        _selectNum = appConfig.SelectNum;
        _startTime = appConfig.StartTime;
        _needSelect = appConfig.NeedSelect;
    }

    public async Task<bool> CheckLogin()
    {
        //检查cookie是否过期
        await page.GotoAsync("https://wap.showstart.com/pages/myHome/myHome",
            new() { WaitUntil = WaitUntilState.NetworkIdle });

        try
        {
            //是否有登录按钮
            var loginBtn = await page.WaitForSelectorAsync(".login-btn", new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
            //未登录 跳转登录页面
            if (loginBtn != null)
            {
                //滑块验证码登录
                return await Login();
            }
        }
        catch (Exception)
        {
            Console.WriteLine("登录成功");
            return true;
        }

        return true;
    }

    public async Task<bool> Login()
    {
        await page.GotoAsync("https://wap.showstart.com/pages/passport/login/login?redirect=%252Fpages%252FmyHome%252FmyHome",
    new() { WaitUntil = WaitUntilState.NetworkIdle });

        await page.WaitForTimeoutAsync(1000 * 50);

        if (await page.TitleAsync() == "我的")
        {
            return true;
        }

        return false;
    }

    public async Task<IElementHandle?> GetPayBtn()
    {
        var confirmUrl = $@"https://wap.showstart.com/pages/order/activity/confirm/confirm?sequence=" +
   $@"{_activityId}&ticketId={_ticketId}&ticketNum={_ticketNum}&ioswx=1&terminal=app&from=singlemessage&isappinstalled=0";

        IElementHandle? payBtn;
        var flashCount = 0; // 刷新次数
        while (true)
        {
            //超过20次循环未获取按钮返回
            if (flashCount >= 10)
            {
                return null;
            }

            await page.GotoAsync(confirmUrl, new() { WaitUntil = WaitUntilState.NetworkIdle });
            try
            {
                payBtn = await page.WaitForSelectorAsync(".payBtn", new() { State = WaitForSelectorState.Visible, Strict = true, Timeout = 5000 });
            }
            catch (Exception)
            {
                Console.WriteLine("获取支付按钮超时");
                continue;
            }
            var text = payBtn == null ? "" : await payBtn.TextContentAsync();
            if (text != null && text.Contains("立即支付"))
            {
                Console.WriteLine("获取支付按钮成功！");
                break;
            }

            flashCount++;
        }

        return payBtn;
    }

    public async Task BuyTicket(IElementHandle payBtn)
    {
        Console.WriteLine($"是否需要选择观演人: {_needSelect}, 如果需要, 选择数量: {_selectNum}");
        if (_needSelect)
        {
            //选择观演人
        }

        Console.WriteLine("开始抢票");
        while (true)
        {
            try
            {
                await payBtn.ClickAsync(new() { ClickCount = 10 });
            }
            catch (Exception e)
            {
                Console.WriteLine("点击支付按钮发生异常，可能是已经抢票成功, 请查看手机 但是先不要退出");
                break;
            }

            await page.WaitForTimeoutAsync(200);
        }

    }

    public async Task SelectPerson()
    {
        try
        {
            var selectBtn = await page.WaitForSelectorAsync(".link-item>.rr>.tips", new() { State = WaitForSelectorState.Visible, Timeout = 200 });
            await selectBtn.ClickAsync();

            for (int i = 0; i < _selectNum; i++)
            {
                var checkbox = await page.WaitForSelectorAsync($".uni-scroll-view-content > uni-checkbox-group > uni-label:nth-child({i + 1})", new() { Timeout = 200 });
                await checkbox.ClickAsync();
            }

            var confirmBtn = await page.WaitForSelectorAsync(".pop-box>.pop-head>uni-view:nth-child(2)", new() { State = WaitForSelectorState.Visible, Timeout = 200 });
            confirmBtn?.ClickAsync();
        }
        catch (Exception)
        {
            Console.WriteLine("未获取到选择观演人按钮");
            await page.ReloadAsync();
        }
    }

    public async Task PurchaseLoop(int loopCount = 10)
    {
        for (int i = 0; i < loopCount; i++)
        {
            var payBtn = await GetPayBtn();

            if (payBtn == null)
            {
                Console.WriteLine("未获取到支付按钮");
                continue;
            }

            await BuyTicket(payBtn);
            await page.WaitForTimeoutAsync(100);
        }

    }
}