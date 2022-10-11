using System;
using Microsoft.Playwright;

namespace PlaywrightDemo;

public class Helper
{
    private string _event;
    private string _ticketId;
    private string _ticketNum;
    private DateTime? _startTime;
    private bool _needSelect;
    private int _selectNum;
    public IPage page;

    public Helper(IPage page,
                  string ticketId,
                  string ticketEvent,
                  bool needSelect,
                  DateTime? startTime,
                  string ticketNum = "1",
                  int selectNum = 1)
    {
        this.page = page;
        _event = ticketEvent;
        _ticketId = ticketId;
        _ticketNum = ticketNum;
        _selectNum = selectNum;
        _startTime = startTime;
        _needSelect = needSelect;
    }

    public async Task<IElementHandle?> LoadTicketPage()
    {
        for (int i = 0; i < 3; i++)
        {
            await page.WaitForTimeoutAsync(1000 * 10);
            if (await page.TitleAsync() == "我的")
            {
                Console.WriteLine("登陆成功");
                break;
            }
        }

        if (await page.TitleAsync() != "我的")
        {
            Console.WriteLine("未登录");
            return null;
        }

        var confirmUrl = $@"https://wap.showstart.com/pages/order/activity/confirm/confirm?sequence=" +
   $@"{_event}&ticketId={_ticketId}&ticketNum={_ticketNum}&ioswx=1&terminal=app&from=singlemessage&isappinstalled=0";

        IElementHandle? payBtn;
        while (true)
        {
            await page.GotoAsync(confirmUrl, new() { WaitUntil = WaitUntilState.NetworkIdle });
            payBtn = await page.WaitForSelectorAsync(".payBtn", new PageWaitForSelectorOptions() { State = WaitForSelectorState.Attached, Strict = true, Timeout = 1000 * 10 });
            var text = payBtn == null ? "" : await payBtn.TextContentAsync();
            if (text != null && text.Contains("立即支付"))
            {
                break;
            }
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

        if (_startTime == null)
        {
            Console.WriteLine("开始抢票");

            while (true)
            {
                try
                {
                    await payBtn.ClickAsync(new() { ClickCount = 10, });
                }
                catch (Exception e)
                {
                    Console.WriteLine("点击支付按钮发生异常，可能是已经抢票成功, 请查看手机 但是先不要退出");
                    continue;
                }
            }
        }
        else
        {

        }
    }
}