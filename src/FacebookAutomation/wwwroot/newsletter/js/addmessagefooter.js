$(document).ready(function() {
	$('#formAddMessageFooter').click(function(){
		//var nom=$('#name').val();
		//var messg=$('#message').val();
		var email = $('#email_address_footer').val();
		//var sujet = $('#subject').val();
		//alert(email+"  bbbbbbbbbbbbbbbbbbbbb");
		$( '#footerSuccess').hide( "fast" );
		var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
		if (!(filter.test(email))) {
			$( '#footerSuccess').show( "fast" );
			$('#footerSuccess').html("<h5 style='color:#FF8000'>Email non valide</h5></div>");
			$('#footerSuccess').focus();
			//$('.error').remove();
//			$('.success').remove();
			return false;
		}
		//show the loading sign
        $('#loaderfooter').show();
		$.ajax({
			type: "post",
			url: "newsletter/newsletter-subscribe-footer.php",
			data: {email_address_footer:email},
			success: function(data)
			{
				//alert(data);
				$('#loaderfooter').hide();
				var json = $.parseJSON(data);
                        //now json variable contains data in json format
                        //let's display a few items
				
				if(json.response=="success")
				{
					$('#footerSuccess').html("<b style='color:#000000'>Merci d\'avoir souscrit a notre newsletter !</b>");
				$( '#footerSuccess').show( 5000 );
					
				}
				else
				{
					
					$('#footerSuccess').html("<b style='color:#000000'>Votre email existe deja !</b>");
				$( '#footerSuccess').show( 5000 );
				}
                //$('#footerSuccess').html('Plugin name: ' + json.response + '<br />Author: ' + json.message);

				
				
				
				//$('#footerSuccess').html("<b style='color:#FF8000'>Merci d\'avoir souscrit a notre newsletter</b>");
				//$( '#footerSuccess').show( "fast" );
				
				$('#email_address_footer').val("");
				//$("#footerSuccess").fadeOut("25000");
				//$('#footerSuccess').before(data);
//				$('.warning').remove();
				
			}
		});
		return false;
	});
	$( ".abhijitscript" ).keyup(function() {
	   var email = $('.abhijitscript').val();
	   if(email.length == 0)
	   {
		$('.error').remove();
		$('.success').remove();
		$('.warning').remove();
	   }
	});
});
