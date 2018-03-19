using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Sample.LogicMine.Types;

namespace Sample.LogicMine.WebApi.Client
{
  /// <summary>
  /// Completely useless and contrived but the hope is that it's easy to follow while 
  /// at the same time not being so trivial that it's pointless.
  /// 
  /// This is basically the same application as Sample.LogicMine.Console but it's calling into a 
  /// service which hosts all mines, rather than hosting them within it's own proces.
  /// </summary>
  public class Genesis
  {
    private readonly HttpClient _client;
    private DateTime _day;

    public Genesis(string user, HttpClient client)
    {
      _client = client ?? throw new ArgumentNullException(nameof(client));
      _day = DateTime.MinValue;

      _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user);
    }

    private static void EnsureSuccess(HttpResponseMessage response)
    {
      if (response == null)
        throw new ArgumentNullException(nameof(response));

      if (response.IsSuccessStatusCode)
        return;

      var message = response.Content.ReadAsStringAsync().Result;
      throw new InvalidOperationException($"Response indicated failure: {message}");
    }

    /// <summary>
    /// Creates a frog universe, i.e. a pond populated with a pair of original frogs.  
    /// These frogs then go forward and multiply, having tadpoles.  These tadpoles will at 
    /// some point mature and then breed over the space of a year.
    /// </summary>
    /// <returns></returns>
    public async Task LetThereBeLightAsync()
    {
      var pond = await CreatePondAsync();
      _day = _day.AddDays(7);

      Inhabit(pond);
      _day = _day.AddMonths(3);

      do
      {
        await Multiply();

        _day = _day.AddDays(7);
      } while (_day < DateTime.MinValue.AddYears(1));

      PrintOrganismCounts();
    }

    private async Task<Pond> CreatePondAsync()
    {
      var frogden = new Pond {Name = "Frogden"};

      using (var response = await _client.PostAsJsonAsync("ponds", frogden))
      {
        EnsureSuccess(response);
        frogden.Id = await response.Content.ReadAsAsync<int>();

        if (frogden.Id <= 0)
          throw new InvalidOperationException("Did not receive pond Id from server");
      }

      return frogden;
    }

    private void Inhabit(Pond pond)
    {
      var frogam = new Frog {Name = "Frogam", IsMale = true, LivesInPondId = pond.Id, DateOfBirth = _day};
      var freve = new Frog {Name = "Freve", IsMale = false, LivesInPondId = pond.Id, DateOfBirth = _day};

      var postFrogamTask = _client.PostAsJsonAsync("frogs", frogam)
        .ContinueWith(t => EnsureSuccess(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);

      var postFreveTask = _client.PostAsJsonAsync("frogs", freve)
        .ContinueWith(t => EnsureSuccess(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);

      Task.WaitAll(postFrogamTask, postFreveTask);
    }

    private async Task Multiply()
    {
      await MatureTadpolesAsync().ConfigureAwait(false);

      using (var response = await _client.GetAsync($"pairSelections/single?filter=date eq {_day:yyyy-MM-dd}"))
      {
        EnsureSuccess(response);

        var pair = await response.Content.ReadAsAsync<PairSelection>();
        if (pair.IsValid)
        {
          var matingEvent = new MatingEvent(pair.Male, pair.Female, pair.Date);
          using (var mateResponse = await _client.PostAsJsonAsync("matingEvents", matingEvent))
          {
            EnsureSuccess(mateResponse);
            
            var message = await mateResponse.Content.ReadAsStringAsync();
            Console.WriteLine(message.Replace("\"", ""));
          }
        }
      }
    }

    private async Task MatureTadpolesAsync()
    {
      using (var response = await _client.PostAsJsonAsync("maturationEvents", new MaturationEvent(_day)))
      {
        EnsureSuccess(response);
        var maturedCount = await response.Content.ReadAsAsync<int>();
        if (maturedCount > 0)
          Console.WriteLine($"[{_day:d}] {maturedCount} tadpoles have matured");
      }
    }

    private void PrintOrganismCounts()
    {
      var getFrogCountTask = _client.GetAsync("frogCounts/single")
        .ContinueWith(async t =>
        {
          using (t.Result)
          {
            EnsureSuccess(t.Result);
            return await t.Result.Content.ReadAsAsync<FrogCount>().ConfigureAwait(false);
          }
        }, TaskContinuationOptions.OnlyOnRanToCompletion);

      var getTadpoleCountTask = _client.GetAsync("tadpoleCounts/single")
        .ContinueWith(async t =>
        {
          using (t.Result)
          {
            EnsureSuccess(t.Result);
            return await t.Result.Content.ReadAsAsync<TadpoleCount>().ConfigureAwait(false);
          }
        }, TaskContinuationOptions.OnlyOnRanToCompletion);

      Console.WriteLine(
        $"There are {getFrogCountTask.Result.Result.Count} frogs and {getTadpoleCountTask.Result.Result.Count} tadpoles");
    }
  }
}
