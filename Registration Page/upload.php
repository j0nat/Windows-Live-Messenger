<?php
$uploads_dir = 'uploads/';

function generateRandomString($length = 10)
{
    return substr(str_shuffle(str_repeat($x='0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ', ceil($length/strlen($x)) )),1,$length);
}

if ($_FILES["file"]["size"] < 5000000) // 5mb size limit
{
	if ($_FILES["file"]["error"] == UPLOAD_ERR_OK &
	isset($_FILES['file']))
	{
		$ext = pathinfo($_FILES['file']['name'], PATHINFO_EXTENSION);
		$tmp_name = $_FILES["file"]["tmp_name"];
		$name = generateRandomString(14) . ".$ext";
		move_uploaded_file($tmp_name, "$uploads_dir/$name");

		
		echo "$name";
	}
	else
	{
		echo "0";
	}
}
?>