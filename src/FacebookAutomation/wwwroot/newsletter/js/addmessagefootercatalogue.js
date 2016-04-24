$(document).ready(function() {
	$('#formAddMessageFootercatalogue').click(function(){
		
		var email = $('#email_address').val();
		//var sujet = $('#subject').val();
		//alert(email);
		$( '#footerSuccessCatalogue').hide( "fast" );
		var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
		if (!(filter.test(email))) {
			$( '#footerSuccessCatalogue').show( "fast" );
			$('#footerSuccessCatalogue').html("<h5 style='color:#FF8000'>Email non valide</h5></div>");
			$('#footerSuccessCatalogue').focus();
			//$('.error').remove();
//			$('.success').remove();
			return false;
		}
		//show the loading sign
        $('#loader').show();
		$.ajax({
			type: "post",
			url: "newsletter/newsletter-subscribe.php",
			data: {email_address:email},
			success: function(data)
			{
				//alert(data);
				$('#loader').hide();
				$('#footerSuccessCatalogue').html("<b style='color:#006666'> Notre catalogue vous a ete envoye . Veuillez verifier votre email</b><br /> ");
				
				$( '#footerSuccessCatalogue').show( 5000 );
				//$('#footerSuccess').html("<b>Votre requete a &eacute;t&eacute; prise en compte</b></div>");
				
				$('#email_address').val("");
				//$("#info").fadeOut("5000");
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
