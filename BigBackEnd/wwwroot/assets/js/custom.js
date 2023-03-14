$(document).ready(function () {
    console.log("test")
    $(document).on("click", '.product-close, .element-delete-trashbin, .basket-product-increase', function (e) {
        e.stopPropagation();
        e.preventDefault();
        console.log(e.currentTarget)

        if (e.currentTarget.classList.contains('product-close') || e.currentTarget.classList.contains('element-delete-trashbin')) {
            let url = $(e.currentTarget).attr('href');

            fetch(url)
                .then(res => {
                    return res.text();
                }).then(data => {
                    $('.header-cart').html(data)
                    console.log(url)
                    let url2 = "/" + url.split('/')[1] + "/mainbasket"
                    console.log(url2)
                    fetch(url2)
                        .then(res2 => {
                            return res2.text()
                        })
                        .then(data2 => {
                            $('.cart-page').html(data2)
                        })
                })
        } else if (e.currentTarget.classList.contains('basket-product-increase') || e.currentTarget.classList.contains('basket-product-decrease')) {
            let url = $(e.currentTarget).attr('href');
            console.log(url);
            fetch(url)
                .then(res => {
                    return res.text();
                }).then(data => {
                    $('.header-cart').html(data)
                    console.log(url)
                    let url2 = "/" + url.split('/')[1] + "/refreshbasketmain"
                    console.log(url2)
                    fetch(url2)
                        .then(res2 => {
                            return res2.text()
                        })
                        .then(data2 => {
                            $('.cart-page').html(data2)
                        })
                })
        } 
            
    })




    //$('.product-close').click(function (e) {
    //    e.preventDefault();

    //    let url = $(this).attr('href');
    //    //basket/removebasket/19
    //    //basket/removefrommain
    //    console.log("aksj")
    //    fetch(url)
    //        .then(res => {
    //            return res.text();
    //        }).then(data => {
    //            $('.header-cart').html(data)
                
    //        })
    //})

    $('.search').keyup(function () {

        let search = $(this).val();
        let categoryId = $('.category').val();

        if (search.length >= 3) {
            fetch('/product/search?search=' + search + '&categoryId=' + categoryId)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.searchBody').html(data)
                })
        } else {
            $('.searchBody').html('');
        }

    })

    $('.productModal').click(function (e) {
        e.preventDefault();

        let url = $(this).attr('href')

        fetch(url)
            .then(res => {
                return res.text()
            }).then(data => {
                $('.modal-content').html(data)
                $('.quick-view-image').slick({
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: false,
                    dots: false,
                    fade: true,
                    asNavFor: '.quick-view-thumb',
                    speed: 400,
                });

                $('.quick-view-thumb').slick({
                    slidesToShow: 4,
                    slidesToScroll: 1,
                    asNavFor: '.quick-view-image',
                    dots: false,
                    arrows: false,
                    focusOnSelect: true,
                    speed: 400,
                });























                //let modal = '<div class="modal-header">
                //    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                //        <i class="fal fa-times"></i>
                //    </button>
                //</div>
                //<div class="modal-body">
                //    <div class="row">
                //        <div class="col-md-6">
                //            <div class="product-quick-view-image mt-30">
                //                <div class="quick-view-image">
                //                    <div class="single-view-image">
                //                        <img src="~/assets/images/product-quick/product-1.jpg" alt="product">
                //                    </div>
                //                    <div class="single-view-image">
                //                        <img src="~/assets/images/product-quick/product-2.jpg" alt="product">
                //                    </div>
                //                    <div class="single-view-image">
                //                        <img src="~/assets/images/product-quick/product-3.jpg" alt="product">
                //                    </div>
                //                    <div class="single-view-image">
                //                        <img src="~/assets/images/product-quick/product-4.jpg" alt="product">
                //                    </div>
                //                    <div class="single-view-image">
                //                        <img src="~/assets/images/product-quick/product-5.jpg" alt="product">
                //                    </div>
                //                </div>
                //                <ul class="quick-view-thumb">
                //                    <li>
                //                        <div class="single-thumb">
                //                            <img src="~/assets/images/product-quick/product-1.jpg" alt="">
                //                        </div>
                //                    </li>
                //                    <li>
                //                        <div class="single-thumb">
                //                            <img src="~/assets/images/product-quick/product-2.jpg" alt="">
                //                        </div>
                //                    </li>
                //                    <li>
                //                        <div class="single-thumb">
                //                            <img src="~/assets/images/product-quick/product-3.jpg" alt="">
                //                        </div>
                //                    </li>
                //                    <li>
                //                        <div class="single-thumb">
                //                            <img src="~/assets/images/product-quick/product-4.jpg" alt="">
                //                        </div>
                //                    </li>
                //                    <li>
                //                        <div class="single-thumb">
                //                            <img src="~/assets/images/product-quick/product-5.jpg" alt="">
                //                        </div>
                //                    </li>
                //                </ul>
                //            </div> <!-- Modal Quick View Image -->
                //        </div>
                //        <div class="col-md-6">
                //            <div class="product-quick-view-content mt-30">
                //                <h3 class="product-title">Trans-Weight Hooded Wind and Water Resistant Shell</h3>
                //                <p class="reference">Reference: demo_12</p>
                //                <ul class="rating">
                //                    <li class="rating-on"><i class="fas fa-star"></i></li>
                //                    <li class="rating-on"><i class="fas fa-star"></i></li>
                //                    <li class="rating-on"><i class="fas fa-star"></i></li>
                //                    <li class="rating-on"><i class="fas fa-star"></i></li>
                //                    <li class="rating-on"><i class="fas fa-star"></i></li>
                //                </ul>
                //                <div class="product-prices">
                //                    <span class="sale-price"> €23.90</span>
                //                    <span class="regular-price">€21.03</span>
                //                    <span class="save">Save 12%</span>
                //                </div>
                //                <p class="product-description">Block out the haters with the fresh adidas® Originals Kaval Windbreaker Jacket. <br> Part of the Kaval Collection. <br> Regular fit is eased, but not sloppy, and perfect for any activity. <br> Plain-woven jacket specifically constructed for freedom of movement.</p>
                //                <div class="product-size-color flex-wrap">
                //                    <div class="product-size">
                //                        <h5 class="title">Size</h5>
                //                        <select>
                //                            <option value="1">S</option>
                //                            <option value="2">M</option>
                //                            <option value="3">L</option>
                //                            <option value="4">XL</option>
                //                        </select>
                //                    </div>
                //                    <div class="product-color">
                //                        <h5 class="title">Color</h5>
                //                        <div class="color-input">
                //                            <div class="single-color color-1">
                //                                <input type="radio" id="radio-1" name="color">
                //                                <label for="radio-1"></label>
                //                            </div>
                //                            <div class="single-color color-2">
                //                                <input type="radio" id="radio-2" name="color">
                //                                <label for="radio-2"></label>
                //                            </div>
                //                            <div class="single-color color-3">
                //                                <input type="radio" id="radio-3" name="color">
                //                                <label for="radio-3"></label>
                //                            </div>
                //                        </div>
                //                    </div>
                //                    <div class="product-quantity">
                //                        <h5 class="title">Quantity</h5>
                //                        <div class="quantity d-flex">
                //                            <button type="button" id="sub" class="sub"><i class="fal fa-minus"></i></button>
                //                            <input type="text" value="1" />
                //                            <button type="button" id="add" class="add"><i class="fal fa-plus"></i></button>
                //                        </div>
                //                    </div>
                //                </div>
                //                <div class="product-add-cart">
                //                    <button><i class="icon ion-bag"></i> Add to cart</button>
                //                </div>
                //                <div class="product-wishlist-compare">
                //                    <ul class="d-flex flex-wrap">
                //                        <li><a href="#"><i class="fal fa-heart"></i> Add to wishlist</a></li>
                //                        <li><a href="#"><i class="fal fa-repeat"></i> Add to compare</a></li>
                //                    </ul>
                //                </div>
                //                <div class="product-share d-flex">
                //                    <p>Share</p>
                //                    <ul class="social media-body">
                //                        <li><a href="#"><i class="fab fa-facebook-f"></i></a></li>
                //                        <li><a href="#"><i class="fab fa-twitter"></i></a></li>
                //                        <li><a href="#"><i class="fab fa-google"></i></a></li>
                //                        <li><a href="#"><i class="fab fa-pinterest-p"></i></a></li>
                //                    </ul>
                //                </div>
                //            </div> <!-- Modal Quick View Content -->
                //        </div>
                //    </div> <!-- row -->
                //</div>  <!-- Modal Body -->'


                //$('.modal-content').html(modal);
            })
    })
})