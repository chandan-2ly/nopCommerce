var AjaxCatalogProducts = {
  settings: {
    fetchUrl: false,
    browserPath: false,
    categoryId: 0,
    manufacturerId: 0,
  },

  params: {
    jqXHR: false,
  },

  init: function (settings) {
    this.settings = $.extend({}, this.settings, settings);

    var self = this;

    var $viewModeEls = $('[data-viewmode]');
    if ($viewModeEls && $viewModeEls.length > 0) {
      $viewModeEls.on('click', function () {
        if (!$(this).hasClass('selected')) {
          $viewModeEls.toggleClass('selected');
          self.getProducts();
        }
        return false;
      });
    }

    var $orderByEl = $('#products-orderby');
    if ($orderByEl) {
      $orderByEl
        .attr('onchange', null).off('change')
        .on('change', function () {
        self.getProducts();
      });
    }

    var $pageSizeEl = $('#products-pagesize');
    if ($pageSizeEl) {
      $pageSizeEl
        .attr('onchange', null).off('change')
        .on('change', function () {
        self.getProducts();
      });
    }

    var $filterEls = $('[data-option-id]');
    if ($filterEls && $filterEls.length > 0) {
      $filterEls.on('change', function () {
        self.getProducts();
      });
    }

    var $pageEls = $('[data-page]');
    if ($pageEls && $pageEls.length > 0) {
      $.each($pageEls, function (i, el) {
        var $el = $(el);
        if ($el.is('a')) $el.removeAttr('href');
      });
      $pageEls.on('click', function () {
        self.getProducts($(this).data('page'));
      });
    }
  },

  getProducts: function (pageNumber) {
    if (this.params.jqXHR && this.params.jqXHR.readyState !== 4) {
      this.params.jqXHR.abort();
    }

    this.setLoadWaiting(1);

    var self = this;

    var queryBuilder = createCatalogQueryBuilder(this.settings.browserPath);

    var $optionEls = $('[data-option-id]');
    if ($optionEls && $optionEls.length > 0) {
      var selectedOptions = $.map($optionEls, function (el) {
        var $optionEl = $(el);
        if ($optionEl.is(':checked')) return $(el).data('option-id')
        return null;
      });

      if (selectedOptions && selectedOptions.length > 0) {
        queryBuilder.addOptions(selectedOptions);
      }
    }

    var $viewModeEl = $('[data-viewmode].selected');
    if ($viewModeEl && $viewModeEl.length > 0) {
      queryBuilder.addViewMode($viewModeEl.data('viewmode'));
    }

    var $pageSizeEl = $('option:selected', '#products-pagesize');
    if ($pageSizeEl) {
      queryBuilder.addPageSize($pageSizeEl.data('pagesize'));
    }

    var $orderEl = $('option:selected', '#products-orderby');
    if ($orderEl && $orderEl.length > 0) {
      queryBuilder.addOrder($orderEl.data('order'));
    }

    if (pageNumber) {
      queryBuilder.addPageNumber(pageNumber);
    }

    this.setBrowserHistory(queryBuilder.build());

    queryBuilder.addBaseUrl(this.settings.fetchUrl);

    this.params.jqXHR = $.ajax({
      cache: false,
      url: queryBuilder.build(),
      type: 'GET',
      success: function (response) {
        // todo: fire event on success
      },
      error: function (jqXHR, textStatus, errorThrown) {

      },
      complete: function (jqXHR, textStatus) {
        self.setLoadWaiting();
      }
    });
  },

  setLoadWaiting(enable) {
    var $busyEl = $('.ajax-products-busy');
    if (enable) {
      $busyEl.show();
    } else {
      $busyEl.hide();
    }
  },

  setBrowserHistory(url) {
    window.history.pushState({ path: url }, '', url);
  }
}

function createCatalogQueryBuilder(baseUrl) {
  return {
    params: {
      baseUrl: baseUrl,
      query: {}
    },

    addBaseUrl: function (baseUrl) {
      this.baseUrl = baseUrl;
      return this;
    },

    addCategory: function (categoryId) {
      this.params.query.categoryId = categoryId;
      return this;
    },

    addManufacturer: function (manufacturerId) {
      this.params.query.manufacturerId = manufacturerId;
      return this;
    },

    addPrices: function (from, to) {
      this.params.query.price = from + '-' + to;
      return this;
    },

    addOptions: function (options) {
      this.params.query.specs = options.join(',');
      return this;
    },

    addPageSize: function (pageSize) {
      this.params.query.pagesize = pageSize;
      return this;
    },

    addPageNumber: function (pageNumber) {
      this.params.query.pagenumber = pageNumber;
      return this;
    },

    addOrder: function (order) {
      this.params.query.orderby = order;
      return this;
    },

    addViewMode: function (viewMode) {
      this.params.query.viewmode = viewMode;
      return this;
    },

    build: function () {
      var query = $.param(this.params.query);
      return baseUrl + '?' + query;
    }
  }
}