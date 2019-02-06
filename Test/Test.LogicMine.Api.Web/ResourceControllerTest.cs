﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LogicMine.Api.Data;
using LogicMine.Api.Data.Sqlite;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;
using LogicMine.Api.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Test.LogicMine.Api.Web.Util;
using Test.LogicMine.Common.Mine;
using Test.LogicMine.Common.Types;
using Xunit;
using BadRequestObjectResult = LogicMine.Api.Web.BadRequestObjectResult;

namespace Test.LogicMine.Api.Web
{
  public class ResourceControllerTest
  {
    private static readonly string DbFilename = $"{Path.GetTempPath()}\\testrc.db";
    private DbGenerator _dbGenerator;

    private GeneralResourceController<int, Frog> GetController(int frogCount)
    {
      var mine = GetMine();
      InsertFrogs(mine, frogCount);

      return new GeneralResourceController<int, Frog>(mine);
    }

    private FrogMine GetMine()
    {
      _dbGenerator = new DbGenerator(DbFilename);
      return new FrogMine(_dbGenerator.CreateDb("FrogId"));
    }

    private void InsertFrogs(FrogMine mine, int count)
    {
      var tasks = new Task[count];
      for (var i = 1; i <= count; i++)
      {
        var frog = new Frog {Name = $"Frank{i}", DateOfBirth = DateTime.Today.AddDays(-i)};
        tasks[i - 1] = mine.SendAsync(new PostBasket<Frog, int>(frog));
      }

      Task.WaitAll(tasks);
    }

    private T UnwrapObjectResult<T>(object result)
    {
      if (!(result is ObjectResult))
        throw new ArgumentException($"'{nameof(result)}' is not an {typeof(ObjectResult)}");

      var jobj = JObject.FromObject(((ObjectResult) result).Value);

      return jobj["result"].ToObject<T>();
    }

    [Fact]
    public async Task Get()
    {
      var controller = GetController(10);

      var response = await controller.GetAsync(5).ConfigureAwait(false);
      var frog = UnwrapObjectResult<Frog>(response);

      Assert.Equal(5, frog.Id);
      Assert.Equal($"Frank{5}", frog.Name);
      Assert.Equal(DateTime.Today.AddDays(-5), frog.DateOfBirth);
    }

    [Fact]
    public async Task GetError()
    {
      var controller = GetController(0);

      var response = await controller.GetAsync(5).ConfigureAwait(false) as InternalServerErrorObjectResult;

      Assert.NotNull(response);
      Assert.Equal(500, response.StatusCode);

      var errors = JObject.Parse((string) response.Value);
      Assert.True(errors.ContainsKey("errors"));
      Assert.True(((JArray) errors["errors"]).Count == 1);
      Assert.Equal("0: No 'Test.LogicMine.Common.Types.Frog' record found.",
        ((JArray) errors["errors"])[0].Value<string>());
    }

    [Fact]
    public async Task GetAll()
    {
      var controller = GetController(100);

      var response = await controller.GetCollectionAsync().ConfigureAwait(false);
      var frogs = UnwrapObjectResult<Frog[]>(response);

      Assert.Equal(100, frogs.Length);
    }
    
    [Fact]
    public async Task GetFiltered()
    {
      var controller = GetController(100);

      var date = DateTime.Today.AddDays(-50).ToString("yyyy-MM-dd");
      var response = await controller.GetCollectionAsync($"dateOfBirth lt {date}").ConfigureAwait(false);
      var frogs = UnwrapObjectResult<Frog[]>(response);

      Assert.Equal(50, frogs.Length);
    }

    [Fact]
    public async Task GetFilteredError()
    {
      var controller = GetController(0);

      var date = DateTime.Today.AddDays(-50).ToString("yyyy-MM-dd");
      var response = await controller.GetCollectionAsync($"dateOfBirth XX {date}").ConfigureAwait(false) as
        InternalServerErrorObjectResult;

      Assert.NotNull(response);
      Assert.Equal(500, response.StatusCode);

      var errors = JObject.Parse((string) response.Value);
      Assert.True(errors.ContainsKey("errors"));
      Assert.True(((JArray) errors["errors"]).Count == 1);
      Assert.Equal("0: Unexpected filter operator: 'XX'.", ((JArray) errors["errors"])[0].Value<string>());
    }

    [Fact]
    public async Task GetPaged()
    {
      var controller = GetController(100);

      for (var i = 0; i < 16; i++)
      {
        var response = await controller.GetCollectionAsync(null, 6, i).ConfigureAwait(false);
        var frogs = UnwrapObjectResult<Frog[]>(response);
        Assert.Equal(6, frogs.Length);
      }

      var finalResponse = await controller.GetCollectionAsync(null, 6, 16).ConfigureAwait(false);
      var frogs2 = UnwrapObjectResult<Frog[]>(finalResponse);
      Assert.Equal(4, frogs2.Length);
    }

    [Fact]
    public async Task GetFilteredPaged()
    {
      var controller = GetController(100);
      var date = DateTime.Today.AddDays(-50).ToString("yyyy-MM-dd");

      for (var i = 0; i < 8; i++)
      {
        var response = await controller.GetCollectionAsync($"dateOfBirth lt {date}", 6, i).ConfigureAwait(false);
        var frogs = UnwrapObjectResult<Frog[]>(response);

        Assert.Equal(6, frogs.Length);
      }

      var finalResponse = await controller.GetCollectionAsync($"dateOfBirth lt {date}", 6, 8).ConfigureAwait(false);
      var frogs2 = UnwrapObjectResult<Frog[]>(finalResponse);
      Assert.Equal(2, frogs2.Length);
    }

    [Fact]
    public async Task Patch()
    {
      var controller = GetController(10);

      var delta = new Dictionary<string, object> {{nameof(Frog.Name), "Patched"}};

      var patchResponse = await controller.PatchAsync(7, delta, false).ConfigureAwait(false);
      Assert.Equal(1, UnwrapObjectResult<int>(patchResponse));

      var getResponse = await controller.GetCollectionAsync().ConfigureAwait(false);
      var frogs = UnwrapObjectResult<Frog[]>(getResponse);

      var seenPatched = false;
      Assert.Equal(10, frogs.Length);
      foreach (var frog in frogs)
      {
        if (frog.Id == 7)
        {
          Assert.Equal("Patched", frog.Name);
          seenPatched = true;
        }
        else
          Assert.Equal($"Frank{frog.Id}", frog.Name);
      }

      Assert.True(seenPatched);
    }

    [Fact]
    public async Task PatchError()
    {
      var controller = GetController(0);

      var delta = new Dictionary<string, object> {{"NoField", "Patched"}};

      var response =
        await controller.PatchAsync(7, delta, false).ConfigureAwait(false) as InternalServerErrorObjectResult;

      Assert.NotNull(response);
      Assert.Equal(500, response.StatusCode);

      var errors = JObject.Parse((string) response.Value);
      Assert.True(errors.ContainsKey("errors"));
      Assert.True(((JArray) errors["errors"]).Count == 1);
      Assert.Equal("0: SQLite Error 1: 'near \"WHERE\": syntax error'..",
        ((JArray) errors["errors"])[0].Value<string>());
    }

    [Fact]
    public async Task Post()
    {
      var controller = GetController(10);

      var newFrog = new Frog {Name = "New One", DateOfBirth = DateTime.Today.AddDays(1)};

      var response = await controller.PostAsync(newFrog, false).ConfigureAwait(false);
      Assert.True(UnwrapObjectResult<int>(response) > 10);

      var getResponse = await controller.GetCollectionAsync().ConfigureAwait(false);
      var frogs = UnwrapObjectResult<Frog[]>(getResponse);

      Assert.Equal(11, frogs.Length);
    }

    [Fact]
    public async Task PostError()
    {
      var controller = GetController(10);

      var newFrog = new Frog();

      var response =
        await controller.PostAsync(newFrog, false).ConfigureAwait(false) as InternalServerErrorObjectResult;

      Assert.NotNull(response);
      Assert.Equal(500, response.StatusCode);

      var errors = JObject.Parse((string) response.Value);
      Assert.True(errors.ContainsKey("errors"));
      Assert.True(((JArray) errors["errors"]).Count == 1);
      Assert.Equal("0: SQLite Error 19: 'NOT NULL constraint failed: Frog.Name'..",
        ((JArray) errors["errors"])[0].Value<string>());
    }
    
    [Fact]
    public async Task PostGet()
    {
      var controller = GetController(10);

      var newFrog = new Frog {Name = "New One", DateOfBirth = DateTime.Today.AddDays(1)};

      var response = await controller.PostAsync(newFrog, true).ConfigureAwait(false);
      var frog = UnwrapObjectResult<Frog>(response);
      Assert.True(frog.Id > 0);
      Assert.Equal(newFrog.Name, frog.Name);
      Assert.Equal(newFrog.DateOfBirth, frog.DateOfBirth);
    }

    [Fact]
    public async Task Delete()
    {
      var controller = GetController(10);

      var deleteResponse = await controller.DeleteAsync(4).ConfigureAwait(false);
      Assert.Equal(1, UnwrapObjectResult<int>(deleteResponse));

      var getResponse = await controller.GetCollectionAsync().ConfigureAwait(false);
      var frogs = UnwrapObjectResult<Frog[]>(getResponse);

      Assert.Equal(9, frogs.Length);
      foreach (var frog in frogs)
        Assert.NotEqual(4, frog.Id);
    }

    [Fact]
    public async Task DeleteCollection()
    {
      var controller = GetController(10);

      var deleteResponse = await controller.DeleteCollectionAsync("id in (2,4,6,8)").ConfigureAwait(false);
      Assert.Equal(4, UnwrapObjectResult<int>(deleteResponse));

      var getResponse = await controller.GetCollectionAsync().ConfigureAwait(false);
      var frogs = UnwrapObjectResult<Frog[]>(getResponse);

      Assert.Equal(6, frogs.Length);
      foreach (var frog in frogs)
      {
        Assert.NotEqual(2, frog.Id);
        Assert.NotEqual(4, frog.Id);
        Assert.NotEqual(6, frog.Id);
        Assert.NotEqual(8, frog.Id);
      }
    }

    private class FrogMine : DataMine<int, Frog>
    {
      private static SqliteMappedLayer<int, Frog> GetTerminalLayer(string connectionString)
      {
        var descriptor = new FrogDescriptor();
        return new SqliteMappedLayer<int, Frog>(connectionString, descriptor, new DbMapper<Frog>(descriptor));
      }

      public FrogMine(string connectionString) :
        base(GetTerminalLayer(connectionString))
      {
      }
    }

    private class FrogDescriptor : SqliteMappedObjectDescriptor<Frog>
    {
      public FrogDescriptor() : base("Frog", "FrogId", nameof(Frog.Id))
      {
      }

      public override string GetMappedColumnName(string propertyName)
      {
        if (propertyName == nameof(Frog.Id))
          return "FrogId";

        return base.GetMappedColumnName(propertyName);
      }
    }
  }
}
